namespace BrainRing.Domain.Entities
{
    public class GameSession : Base.Base
    {
        public Guid HostId { get; set; }
        public ICollection<GameSessionUser> Participants { get; set; }
        public ICollection<Question> Questions { get; set; }
        public Guid? CurrentQuestionId { get; set; }
        public Question? CurrentQuestion { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
