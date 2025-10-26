using BrainRing.Application.Interfaces.Repositories;
using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Models.Answer;
using BrainRing.Application.Models.GameSession;
using BrainRing.Application.Params.Answer;
using BrainRing.Domain.Entities;

namespace BrainRing.Application.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IGameSessionsRepository _gameSessionRepo;
        private readonly IAnswersRepository _answersRepo;
        private readonly IUsersRepository _usersRepo;
        private readonly IQuestionRepository _questionRepo;

        public AnswerService(
            IGameSessionsRepository gameSessionRepo,
            IAnswersRepository answersRepo,
            IUsersRepository usersRepo,
            IQuestionRepository questionRepo)
        {
            _gameSessionRepo = gameSessionRepo;
            _answersRepo = answersRepo;
            _usersRepo = usersRepo;
            _questionRepo = questionRepo;
        }

        public async Task<AnswerResult> SubmitAnswerAsync(SubmitAnswerParams @params, CancellationToken token = default)
        {
            var session = await _gameSessionRepo.FindByIdAsync(@params.GameSessionId, token, true, r => r.Participants);

            if (session == null || !session.IsActive)
                throw new InvalidOperationException("Сессия не найдена или не активна");


            var question = await _questionRepo.FindOneByConditionAsync(
                q => q.CurrentQuestions.Count > 0 && q.CurrentQuestions.Any(cq => cq.Id == @params.GameSessionId),
                token,
                false,
                [q => q.CurrentQuestions, q => q.Answers]
            );

            if (question == null)
                throw new InvalidOperationException("Нет текущего вопроса");

            var user = await _usersRepo.FindByIdAsync(@params.UserId, token);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            var existingAnswer = question.Answers
                .FirstOrDefault(a => a.UserId == @params.UserId);

            if (existingAnswer != null)
                throw new InvalidOperationException("Вы уже ответили на этот вопрос");

            var isCorrect = question.CorrectOptionIndex == @params.SelectedOptionIndex;

            var answer = new Answer
            {
                UserId = @params.UserId,
                QuestionId = question.Id,
                SelectedOptionIndex = @params.SelectedOptionIndex,
                IsCorrect = isCorrect,
            };

            await _answersRepo.CreateAsync(answer, token);

            if (isCorrect)
            {
                question = await _questionRepo.FindOneByConditionAsync(
                    q => q.Id == question.Id,
                    token,
                    false,
                    [q => q.CurrentQuestions, q => q.Answers]
                );

                var firstCorrect = !question.Answers.Any(a => a.IsCorrect && a.CreatedAt < answer.CreatedAt);

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


            var participants = new List<ParticipantResult>();

            foreach (var p in session.Participants)
            {
                var participant = await _usersRepo.FindByIdAsync(p.UserId);
                participants.Add(new ParticipantResult
                {
                    Id = p.UserId,
                    Name = participant.Name ?? "Unknown",
                    Score = p.Score
                });
            }

            return new AnswerResult
            {
                Participants = participants
            };
        }
    }

}
