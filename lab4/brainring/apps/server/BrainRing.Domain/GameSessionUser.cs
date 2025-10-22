namespace BrainRing.Domain
{
    public class GameSessionUser
    {
        public Guid GameSessionId { get; set; }
        public Guid UserId { get; set; }
        public int Score { get; set; }
    }
}
