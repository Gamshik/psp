using BrainRing.Application.Interfaces.Repositories;
using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Models.GameSession;
using BrainRing.Application.Params.GameSession;
using BrainRing.Domain.Entities;

namespace BrainRing.Application.Services
{
    public class GameSessionService : IGameSessionService
    {
        private readonly IGameSessionsRepository _gameSessionRepo;
        private readonly IUsersRepository _usersRepo;

        public GameSessionService(IGameSessionsRepository gameSessionRepo, IUsersRepository usersRepo)
        {
            _gameSessionRepo = gameSessionRepo;
            _usersRepo = usersRepo;
        }

        public async Task<GameSessionResult> CreateGameSessionAsync(CreateGameSessionParams @params, CancellationToken token = default)
        {
            var session = new GameSession
            {
                Id = Guid.NewGuid(),
                HostId = @params.HostId,
                Participants = @params.ParticipantIds.Select(id => new GameSessionUser { UserId = id }).ToList(),
                IsActive = true
            };

            await _gameSessionRepo.CreateAsync(session, token);

            return await MapToResultAsync(session);
        }

        public async Task<GameSessionResult> JoinGameSessionAsync(JoinGameSessionParams @params, CancellationToken token = default)
        {
            var session = await _gameSessionRepo.FindByIdAsync(@params.GameSessionId, token, true);

            if (session == null || !session.IsActive)
                throw new InvalidOperationException("Комната не найдена или не активна");

            if (session.Participants.Any(p => p.UserId == @params.UserId))
                throw new InvalidOperationException("Пользователь уже в комнате");

            session.Participants.Add(new GameSessionUser { UserId = @params.UserId });

            await _gameSessionRepo.UpdateAsync(session, token);

            return await MapToResultAsync(session);
        }

        public async Task<GameSessionResult?> GetGameSessionAsync(Guid sessionId, CancellationToken token = default)
        {
            var session = await _gameSessionRepo.FindByIdAsync(sessionId, token, true);
            if (session == null) return null;

            return await MapToResultAsync(session);
        }

        public async Task<GameSessionResult?> NextQuestionAsync(Guid sessionId, CancellationToken token = default)
        {
            var session = await _gameSessionRepo.FindByIdAsync(sessionId, token, true);
            if (session == null || !session.IsActive) return null;

            var nextQuestion = session.Questions.FirstOrDefault(q => session.CurrentQuestionId == null || q.Id != session.CurrentQuestionId);

            session.CurrentQuestionId = nextQuestion?.Id;

            await _gameSessionRepo.UpdateAsync(session, token);

            return await MapToResultAsync(session);
        }

        private async Task<GameSessionResult> MapToResultAsync(GameSession session)
        {
            var participants = new List<ParticipantResult>();

            foreach (var p in session.Participants)
            {
                var user = await _usersRepo.FindByIdAsync(p.UserId);
                participants.Add(new ParticipantResult
                {
                    Id = p.UserId,
                    Name = user?.Name ?? "Unknown",
                    Score = p.Score
                });
            }

            QuestionResult? currentQuestion = null;
            if (session.CurrentQuestion != null)
            {
                currentQuestion = new QuestionResult
                {
                    Id = session.CurrentQuestion.Id,
                    Text = session.CurrentQuestion.Text,
                    Options = session.CurrentQuestion.Options
                        .Select(o => new QuestionOptionResult { Id = o.Id, Title = o.Title })
                        .ToList()
                };
            }

            return new GameSessionResult
            {
                Id = session.Id,
                HostId = session.HostId,
                Participants = participants,
                CurrentQuestion = currentQuestion,
                IsActive = session.IsActive
            };
        }
    }

}
