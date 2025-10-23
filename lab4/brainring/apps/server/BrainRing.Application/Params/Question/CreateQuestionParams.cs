namespace BrainRing.Application.Params.Question
{
    public class CreateQuestionParams
    {
        public Guid GameSessionId { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; } = new();
        public int CorrectOptionIndex { get; set; }
    }
}
