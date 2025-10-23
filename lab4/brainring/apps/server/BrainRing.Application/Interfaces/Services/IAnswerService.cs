using BrainRing.Application.Models.Answer;
using BrainRing.Application.Params.Answer;

namespace BrainRing.Application.Interfaces.Services
{
    public interface IAnswerService
    {
        Task<AnswerResult> SubmitAnswerAsync(SubmitAnswerParams @params, CancellationToken token = default);
    }
}
