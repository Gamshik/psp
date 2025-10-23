namespace BrainRing.Domain.Entities
{
    public class Answer : Base.Base
    {
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public int SelectedOptionIndex { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
