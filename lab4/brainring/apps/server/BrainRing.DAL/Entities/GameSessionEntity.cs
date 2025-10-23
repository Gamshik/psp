using BrainRing.DbAdapter.Entities.Base;

namespace BrainRing.DbAdapter.Entities
{
    public class GameSessionEntity : BaseEntity
    {
        public Guid HostId { get; set; }
        public UserEntity Host { get; set; }
        public ICollection<GameSessionUserEntity> Participants { get; set; }
        public ICollection<QuestionEntity> Questions { get; set; }
        public Guid? CurrentQuestionId { get; set; }
        public QuestionEntity CurrentQuestion { get; set; }
        public bool IsActive { get; set; }
    }
}
