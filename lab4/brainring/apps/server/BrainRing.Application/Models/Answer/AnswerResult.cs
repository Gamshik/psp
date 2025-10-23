namespace BrainRing.Application.Models.Answer
{
    public class AnswerResult
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsCorrect { get; set; }
        public int Score { get; set; }
    }
}
