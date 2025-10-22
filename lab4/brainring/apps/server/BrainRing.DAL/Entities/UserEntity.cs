namespace BrainRing.DAL.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<GameSessionUserEntity> GameSessions { get; set; }
        public ICollection<GameSessionEntity> HostSessions { get; set; }
        public ICollection<AnswerEntity> Answers { get; set; }
    }
}
