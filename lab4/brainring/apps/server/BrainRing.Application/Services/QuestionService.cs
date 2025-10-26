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
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IGameSessionsRepository gameSessionRepo, IQuestionRepository questionRepository)
        {
            _gameSessionRepo = gameSessionRepo;
            _questionRepository = questionRepository;
        }

        public async Task<QuestionResult> CreateQuestionAsync(CreateQuestionParams @params, CancellationToken token = default)
        {
            var session = await _gameSessionRepo.FindByIdAsync(@params.GameSessionId, token, true, [r => r.Questions, r => r.CurrentQuestion]);
           
            if (session == null || !session.IsActive)
                throw new InvalidOperationException("Сессия не найдена или не активна");

            var last = session.Questions.FirstOrDefault(q => q.CurrentQuestions.Count > 0);

            if (last != null)
            {
                last.CurrentQuestions = new List<GameSession>();
                await _questionRepository.UpdateAsync(last, token);
                
                session = await _gameSessionRepo.FindByIdAsync(@params.GameSessionId, token, true, [r => r.Questions, r => r.CurrentQuestion]);

            }

            var question = new Question
            {
                GameSessionId = session.Id,
                Text = @params.Text,
                CorrectOptionIndex = @params.CorrectOptionIndex,
                Options = @params.Options.Select(o => new QuestionOption { Title = o }).ToList(),
                CurrentQuestions = new List<GameSession>(),
            };

            var newQuestion = await _questionRepository.CreateAsync(question);

            if (newQuestion == null)
                throw new InvalidOperationException("Ошибка создания вопроса");

            newQuestion.CurrentQuestions.Add(session); 
            await _questionRepository.SaveChangesAsync();

            session.Questions.Add(newQuestion);

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
