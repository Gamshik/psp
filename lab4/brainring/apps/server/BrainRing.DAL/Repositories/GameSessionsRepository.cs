using BrainRing.Application.Interfaces.Repositories;
using BrainRing.DbAdapter.Entities;
using BrainRing.DbAdapter.Interfaces.Mappers;
using BrainRing.DbAdapter.Repositories.Base;
using BrainRing.Domain.Entities;

namespace BrainRing.DbAdapter.Repositories
{
    public class GameSessionsRepository : BaseRepository<GameSession, GameSessionEntity>, IGameSessionsRepository
    {
        public GameSessionsRepository(BrainRingDbContext context, IEntityMapper mapper) : base(context, mapper) { }
    }
}
