namespace BrainRing.Domain.Entities
{
    public class Question : Base.Base
    {
        public string Text { get; set; }
        public Guid GameSessionId { get; set; }
        public ICollection<QuestionOption> Options { get; set; }
        public int CorrectOptionIndex { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<GameSession> CurrentQuestions { get; set; }
    }
}
