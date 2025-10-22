namespace BrainRing.Domain
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid GameSessionId { get; set; }
        public ICollection<QuestionOption> Options { get; set; }
        public int CorrectOptionIndex { get; set; }
    }
}
