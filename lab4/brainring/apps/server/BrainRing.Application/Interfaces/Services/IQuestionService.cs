using BrainRing.Application.Models.GameSession;
using BrainRing.Application.Params.Question;

namespace BrainRing.Application.Interfaces.Services
{
    public interface IQuestionService
    {
        Task<QuestionResult> CreateQuestionAsync(CreateQuestionParams @params, CancellationToken token = default);
        Task<QuestionResult?> GetCurrentQuestionAsync(Guid gameSessionId, CancellationToken token = default);
    }
}
