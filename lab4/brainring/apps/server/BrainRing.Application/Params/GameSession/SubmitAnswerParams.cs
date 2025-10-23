namespace BrainRing.Application.Params.GameSession
{
    public class SubmitAnswerParams
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public int SelectedOptionIndex { get; set; }
    }
}
