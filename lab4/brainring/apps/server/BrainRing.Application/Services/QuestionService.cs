using BrainRing.Application.Interfaces.Repositories;
using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Models.GameSession;
using BrainRing.Application.Params.Question;
using BrainRing.Domain.Entities;

namespace BrainRing.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IGameSessionsRepository _gameSessionRepo;

        public QuestionService(IGameSessionsRepository gameSessionRepo)
        {
            _gameSessionRepo = gameSessionRepo;
        }

        public async Task<QuestionResult> CreateQuestionAsync(CreateQuestionParams @params, CancellationToken token = default)
        {
            var session = await _gameSessionRepo.FindByIdAsync(@params.GameSessionId, token, true);
            if (session == null || !session.IsActive)
                throw new InvalidOperationException("Сессия не найдена или не активна");

            var question = new Question
            {
                Id = Guid.NewGuid(),
                GameSessionId = session.Id,
                Text = @params.Text,
                CorrectOptionIndex = @params.CorrectOptionIndex,
                Options = @params.Options.Select(o => new QuestionOption { Id = Guid.NewGuid(), Title = o }).ToList()
            };

            session.Questions.Add(question);

            // Если это первый вопрос, сразу делаем его текущим
            if (session.CurrentQuestionId == null)
                session.CurrentQuestionId = question.Id;

            await _gameSessionRepo.UpdateAsync(session, token);

            return MapToResult(question);
        }

        public async Task<QuestionResult?> GetCurrentQuestionAsync(Guid gameSessionId, CancellationToken token = default)
        {
            var session = await _gameSessionRepo.FindByIdAsync(gameSessionId, token, true);
            if (session == null || session.CurrentQuestion == null) return null;

            return MapToResult(session.CurrentQuestion);
        }

        private QuestionResult MapToResult(Question question)
        {
            return new QuestionResult
            {
                Id = question.Id,
                Text = question.Text,
                Options = question.Options
                    .Select(o => new QuestionOptionResult { Id = o.Id, Title = o.Title })
                    .ToList()
            };
        }
    }
}
