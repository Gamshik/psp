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

        public async Task HandleConnectionAsync(WebSocket socket, Guid userId, Guid? sessionId = null)
        {
            var id = _manager.AddSocket(socket, userId, sessionId);

            using var scope = _scopeFactory.CreateScope();
            var answerService = scope.ServiceProvider.GetRequiredService<IAnswerService>();
            var gameSessionService = scope.ServiceProvider.GetRequiredService<IGameSessionService>();

            if (sessionId.HasValue)
            {
                try
                {
                    var joinParams = new JoinGameSessionParams
                    {
                        GameSessionId = sessionId.Value,
                        UserId = userId
                    };

                    var sessionResult = await gameSessionService.JoinGameSessionAsync(joinParams);

                    await BroadcastToSession(sessionId.Value, new WsMessage
                    {
                        Type = MessageType.NewParticipant,
                        Payload = new
                        {
                            UserId = userId,
                            Name = sessionResult.Participants.FirstOrDefault(p => p.Id == userId)?.Name
                        }
                    });
                }
                catch (Exception ex)
                {
                    await SendMessageAsync(socket, new WsMessage
                    {
                        Type = MessageType.Error,
                        Payload = ex.Message
                    });
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
                            CreateQuestionParams createPayload = JsonSerializer.Deserialize<CreateQuestionParams>(wsMessage.Payload.ToString());
                            createPayload.GameSessionId = sessionId.Value;

                            using (var opScope = _scopeFactory.CreateScope())
                            {
                                var questionService = opScope.ServiceProvider.GetRequiredService<IQuestionService>();
                                Console.WriteLine($"PAYLOAD: {questionService} {123} ");

                                await questionService.CreateQuestionAsync(createPayload);

                            }

                            await BroadcastNewQuestion(sessionId!.Value, wsMessage.Payload);
                            break;

                        case MessageType.AnswerResult:
                            await HandleAnswer(answerService, userId, sessionId!.Value, wsMessage.Payload);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await SendMessageAsync(socket, new WsMessage { Type = MessageType.Error, Payload = ex.Message });
                }

                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await _manager.RemoveSocketAsync(id);
            Console.WriteLine($"❌ Клиент {userId} отключён");
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

        private async Task HandleAnswer(IAnswerService answerService, Guid userId, Guid sessionId, object payload)
        {
            var answerParams = JsonSerializer.Deserialize<BrainRing.Application.Params.Answer.SubmitAnswerParams>(payload.ToString());
            if (answerParams == null) return;

            answerParams.UserId = userId;
            answerParams.GameSessionId = sessionId;

            var result = await answerService.SubmitAnswerAsync(answerParams);

            await BroadcastToSession(sessionId, new WsMessage
            {
                Type = MessageType.AnswerResult,
                Payload = result
            });
        }

        private async Task SendMessageAsync(WebSocket socket, WsMessage message)
        {
            var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            if (socket.State == WebSocketState.Open)
                await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
