namespace BrainRing.Application.Params.Answer
{
    public class SubmitAnswerParams
    {
        public Guid GameSessionId { get; set; }
        public Guid UserId { get; set; }
        public int SelectedOptionIndex { get; set; }
    }
}
