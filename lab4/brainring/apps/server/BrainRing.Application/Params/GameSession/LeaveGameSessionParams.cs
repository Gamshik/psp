namespace BrainRing.Application.Params.GameSession
{
    public class LeaveGameSessionParams
    {
        public Guid UserId { get; set; }
        public Guid GameSessionId { get; set; }
    }
}
