using BrainRing.Application.Models.GameSession;

namespace BrainRing.Application.Models.Answer
{
    public class AnswerResult
    {
        public List<ParticipantResult> Participants { get; set; } = new();
    }
}
