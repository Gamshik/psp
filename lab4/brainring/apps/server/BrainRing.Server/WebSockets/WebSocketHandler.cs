using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Params.GameSession;
using BrainRing.Application.Params.Question;
using BrainRing.Server.WebSockets.Enum;
using BrainRing.Server.WebSockets.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace BrainRing.Server.WebSockets
{
    public class WebSocketHandler
    {
        private readonly WebSocketConnectionManager _manager;
        private readonly IServiceScopeFactory _scopeFactory;

        public WebSocketHandler(WebSocketConnectionManager manager, IServiceScopeFactory scopeFactory)
        {
            _manager = manager;
            _scopeFactory = scopeFactory;
        }

        public async Task HandleConnectionAsync(WebSocket socket, Guid userId, Guid? sessionId = null, bool isHost = false)
        {
            var socketId = _manager.AddSocket(socket, userId, sessionId);

            if (sessionId.HasValue)
            {
                try
                {
                    await TryConnectToGame(isHost, sessionId, userId);
                }
                catch (Exception ex)
                {
                    await _manager.RemoveSocketAsync(socketId);
                }
            }

            var buffer = new byte[1024 * 4];
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                try
                {
                    var wsMessage = JsonSerializer.Deserialize<WsMessage>(message);

                    if (wsMessage == null) throw new Exception("Неверный формат сообщения");

                    switch (wsMessage.Type)
                    {
                        case MessageType.NewQuestion:
                            await HandleQuestion(sessionId.Value, wsMessage);
                            break;

                        case MessageType.AnswerResult:
                            await HandleAnswer(userId, sessionId!.Value, wsMessage.Payload);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await SendMessageAsync(socket, new WsMessage { Type = MessageType.Error, Payload = ex.Message });
                }

                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await LeaveFromGame(isHost, sessionId, userId);

            await _manager.RemoveSocketAsync(socketId);
        }

        private async Task TryConnectToGame(bool isHost, Guid? sessionId, Guid userId)
        {
            using var scope = _scopeFactory.CreateScope();
            var gameSessionService = scope.ServiceProvider.GetRequiredService<IGameSessionService>();

            if (!isHost)
            {
                var joinParams = new JoinGameSessionParams
                {
                    GameSessionId = sessionId.Value,
                    UserId = userId
                };

                var sessionResult = await gameSessionService.JoinGameSessionAsync(joinParams);

                await BroadcastToSession(sessionId.Value, new WsMessage
                {
                    Type = MessageType.UpdateParticipants,
                    Payload = new
                    {
                        Participants = sessionResult.Participants,
                    }
                });
            }
            else
            {
                var session = await gameSessionService.GetGameSessionAsync(sessionId.Value, CancellationToken.None);

                if (session == null || !session.IsActive)
                    throw new Exception("Неверная сессия");
            }
        }

        private async Task LeaveFromGame(bool isHost, Guid? sessionId, Guid userId)
        {
            using var scope = _scopeFactory.CreateScope();
            var gameSessionService = scope.ServiceProvider.GetRequiredService<IGameSessionService>();

            if (isHost)
            {
                await gameSessionService.CloseGameSessionAsync(new CloseGameSessionParams { GameSessionId = sessionId.Value }, CancellationToken.None);

                await BroadcastToSession(sessionId.Value, new WsMessage
                {
                    Type = MessageType.CloseGame
                });
            }
            else
            {
                var sessionResult = await gameSessionService.LeaveGameSessionAsync(new LeaveGameSessionParams { UserId = userId, GameSessionId = sessionId.Value }, CancellationToken.None);

                await BroadcastToSession(sessionId.Value, new WsMessage
                {
                    Type = MessageType.UpdateParticipants,
                    Payload = new
                    {
                        Participants = sessionResult.Participants,
                    }
                });
            }
        }

        private async Task BroadcastToSession(Guid sessionId, WsMessage message)
        {
            var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            foreach (var socketPair in _manager.GetAllBySession(sessionId))
            {
                var socket = socketPair.Value;
                if (socket.State == WebSocketState.Open)
                    await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task BroadcastNewQuestion(Guid sessionId, object payload)
        {
            await BroadcastToSession(sessionId, new WsMessage
            {
                Type = MessageType.NewQuestion,
                Payload = payload
            });
        }

        private async Task HandleAnswer(Guid userId, Guid sessionId, object payload)
        {
            var answerParams = JsonSerializer.Deserialize<BrainRing.Application.Params.Answer.SubmitAnswerParams>(payload.ToString());

            if (answerParams == null) return;

            answerParams.UserId = userId;
            answerParams.GameSessionId = sessionId;

            using var opScope = _scopeFactory.CreateScope();
            
            var answerService = opScope.ServiceProvider.GetRequiredService<IAnswerService>();

            var result = await answerService.SubmitAnswerAsync(answerParams);

            await BroadcastToSession(sessionId, new WsMessage
            {
                Type = MessageType.AnswerResult,
                Payload = result
            });
        }

        private async Task HandleQuestion(Guid sessionId, WsMessage wsMessage)
        {
            CreateQuestionParams createPayload = JsonSerializer.Deserialize<CreateQuestionParams>(wsMessage.Payload.ToString());

            if (createPayload == null) return;

            createPayload.GameSessionId = sessionId;

            using var opScope = _scopeFactory.CreateScope(); 

            var questionService = opScope.ServiceProvider.GetRequiredService<IQuestionService>();

            await questionService.CreateQuestionAsync(createPayload);

            await BroadcastNewQuestion(sessionId, wsMessage.Payload);
        }

        private async Task SendMessageAsync(WebSocket socket, WsMessage message)
        {
            var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            if (socket.State == WebSocketState.Open)
                await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
