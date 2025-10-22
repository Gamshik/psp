namespace BrainRing.DAL.Entities
{
    public class QuestionEntity
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid GameSessionId { get; set; }
        public GameSessionEntity GameSession { get; set; }
        public ICollection<QuestionOptionEntity> Options { get; set; }
        public int CorrectOptionIndex { get; set; }
        public ICollection<AnswerEntity> Answers { get; set; }
        public ICollection<GameSessionEntity> CurrentQuestions { get; set; }
    }
}
