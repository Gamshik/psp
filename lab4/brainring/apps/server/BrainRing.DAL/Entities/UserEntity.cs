using BrainRing.DbAdapter.Entities.Base;

namespace BrainRing.DbAdapter.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<GameSessionUserEntity> GameSessions { get; set; }
        public ICollection<GameSessionEntity> HostSessions { get; set; }
        public ICollection<AnswerEntity> Answers { get; set; }
    }
}
