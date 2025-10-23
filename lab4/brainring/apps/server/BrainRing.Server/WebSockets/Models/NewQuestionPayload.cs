namespace BrainRing.Server.WebSockets.Models
{
    public class NewQuestionPayload
    {
        public Guid QuestionId { get; set; }
        public string Text { get; set; }
        public string[] Options { get; set; }
    }
}
