using BrainRing.Domain.Interfaces.Base;

namespace BrainRing.Domain.Entities.Base
{
    public class Base : IBase
    {
        public Guid Id { get; set; }
    }
}
