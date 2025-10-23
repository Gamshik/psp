using AutoMapper;
using BrainRing.DbAdapter.Entities;
using BrainRing.Domain.Entities;

namespace BrainRing.DbAdapter.Mappers.Profiles
{
    public class GameSessionProfile : Profile
    {
        public GameSessionProfile()
        {
            CreateMap<GameSession, GameSessionEntity>().ReverseMap();
        }
    }
}
