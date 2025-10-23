namespace BrainRing.Application.Models.GameSession
{
    public class GameSessionResult
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public List<ParticipantResult> Participants { get; set; } = new();
        public QuestionResult? CurrentQuestion { get; set; }
        public bool IsActive { get; set; }
    }
}
