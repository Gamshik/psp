namespace BrainRing.Domain
{
    public class Answer
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public int SelectedOptionIndex { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
