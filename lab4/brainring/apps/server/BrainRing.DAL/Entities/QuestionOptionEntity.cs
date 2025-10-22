namespace BrainRing.DAL.Entities
{
    public class QuestionOptionEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid QuestionId { get; set; }
        public QuestionEntity Question { get; set; }
    }
}
