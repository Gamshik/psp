namespace BrainRing.DAL.Entities
{
    public class AnswerEntity
    {
        public Guid Id { get; set; }

        // TODO: юзер + куэшенс = индекс
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid QuestionId { get; set; }
        public QuestionEntity Question { get; set; }

        public int SelectedOptionIndex { get; set; }
        public bool IsCorrect { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
