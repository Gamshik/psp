using BrainRing.DbAdapter.Entities.Base;

namespace BrainRing.DbAdapter.Entities
{
    public class GameSessionUserEntity : BaseEntity
    {
        public Guid GameSessionId { get; set; }
        public GameSessionEntity GameSession { get; set; }
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        public int Score { get; set; }
    }
}
