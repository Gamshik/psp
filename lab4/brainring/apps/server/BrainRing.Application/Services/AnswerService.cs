using BrainRing.Application.Interfaces.Repositories;
using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Models.Answer;
using BrainRing.Application.Params.Answer;
using BrainRing.Domain.Entities;

namespace BrainRing.Application.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IGameSessionsRepository _gameSessionRepo;
        private readonly IAnswersRepository _answersRepo;
        private readonly IUsersRepository _usersRepo;

        public AnswerService(
            IGameSessionsRepository gameSessionRepo,
            IAnswersRepository answersRepo,
            IUsersRepository usersRepo)
        {
            _gameSessionRepo = gameSessionRepo;
            _answersRepo = answersRepo;
            _usersRepo = usersRepo;
        }

        public async Task<AnswerResult> SubmitAnswerAsync(SubmitAnswerParams @params, CancellationToken token = default)
        {
            var session = await _gameSessionRepo.FindByIdAsync(@params.GameSessionId, token, true);

            if (session == null || !session.IsActive)
                throw new InvalidOperationException("Сессия не найдена или не активна");

            if (session.CurrentQuestion == null)
                throw new InvalidOperationException("Нет текущего вопроса");

            var user = await _usersRepo.FindByIdAsync(@params.UserId, token);
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            var existingAnswer = session.CurrentQuestion.Answers
                .FirstOrDefault(a => a.UserId == @params.UserId);

            if (existingAnswer != null)
                throw new InvalidOperationException("Вы уже ответили на этот вопрос");

            var isCorrect = session.CurrentQuestion.CorrectOptionIndex == @params.SelectedOptionIndex;

            var answer = new Answer
            {
                Id = Guid.NewGuid(),
                UserId = @params.UserId,
                QuestionId = session.CurrentQuestion.Id,
                SelectedOptionIndex = @params.SelectedOptionIndex,
                IsCorrect = isCorrect,
                CreatedAt = DateTime.UtcNow
            };

            session.CurrentQuestion.Answers.Add(answer);
            await _answersRepo.CreateAsync(answer, token);

            if (isCorrect)
            {
                var firstCorrect = !session.CurrentQuestion.Answers.Any(a => a.IsCorrect && a.CreatedAt < answer.CreatedAt);

                if (firstCorrect)
                {
                    var participant = session.Participants.FirstOrDefault(p => p.UserId == user.Id);
                    if (participant != null)
                    {
                        participant.Score += 1; 
                        await _gameSessionRepo.UpdateAsync(session, token);
                    }
                }
            }

            return new AnswerResult
            {
                UserId = user.Id,
                UserName = user.Name,
                IsCorrect = isCorrect,
                Score = session.Participants.FirstOrDefault(p => p.UserId == user.Id)?.Score ?? 0
            };
        }
    }

}
