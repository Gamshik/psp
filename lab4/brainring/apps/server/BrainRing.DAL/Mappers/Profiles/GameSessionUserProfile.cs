using AutoMapper;
using BrainRing.DbAdapter.Entities;
using BrainRing.Domain.Entities;

namespace BrainRing.DbAdapter.Mappers.Profiles
{
    public class GameSessionUserProfile : Profile
    {
        public GameSessionUserProfile()
        {
            CreateMap<GameSessionUser, GameSessionUserEntity>().ReverseMap();
        }
    }
}
