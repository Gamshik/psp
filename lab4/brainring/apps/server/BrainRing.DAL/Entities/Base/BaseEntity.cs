using BrainRing.DbAdapter.Interfaces.Entities;

namespace BrainRing.DbAdapter.Entities.Base
{
    public class BaseEntity : IBaseEntity
    {
        public Guid Id { get; set; }
    }
}
