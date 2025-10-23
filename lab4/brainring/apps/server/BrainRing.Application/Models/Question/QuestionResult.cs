using BrainRing.Application.Models.GameSession;

namespace BrainRing.Application.Models.Question
{
    public class QuestionResult
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<QuestionOptionResult> Options { get; set; } = new();
    }
}
