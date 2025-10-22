namespace BrainRing.Domain
{
    public class GameSession
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public ICollection<GameSessionUser> Participants { get; set; }
        public ICollection<Question> Questions { get; set; }
        public Question? CurrentQuestion { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
