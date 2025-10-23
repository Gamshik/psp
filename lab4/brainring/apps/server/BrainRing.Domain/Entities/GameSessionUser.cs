namespace BrainRing.Domain.Entities
{
    public class GameSessionUser : Base.Base
    {
        public Guid GameSessionId { get; set; }
        public Guid UserId { get; set; }
        public int Score { get; set; }
    }
}
