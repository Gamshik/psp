using BrainRing.DbAdapter.Entities.Base;

namespace BrainRing.DbAdapter.Entities
{
    public class AnswerEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid QuestionId { get; set; }
        public QuestionEntity Question { get; set; }

        public int SelectedOptionIndex { get; set; }
        public bool IsCorrect { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
