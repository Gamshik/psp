namespace BrainRing.DAL.Entities
{
    public class GameSessionUserEntity
    {
        // TODO: юзер + сессия = индекс
        public Guid GameSessionId { get; set; }
        public GameSessionEntity GameSession { get; set; }
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        public int Score { get; set; }
    }
}
