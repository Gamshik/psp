namespace BrainRing.Application.Models.GameSession
{
    public class QuestionResult
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<QuestionOptionResult> Options { get; set; } = new();
    }
}
