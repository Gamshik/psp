using BrainRing.DbAdapter.Entities.Base;

namespace BrainRing.DbAdapter.Entities
{
    public class QuestionOptionEntity : BaseEntity
    {
        public string Title { get; set; }
        public Guid QuestionId { get; set; }
        public QuestionEntity Question { get; set; }
    }
}
