namespace BrainRing.Domain.Entities
{
    public class QuestionOption : Base.Base
    {
        public string Title { get; set; }
        public Guid QuestionId { get; set; }
    }
}
