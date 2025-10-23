using AutoMapper;
using BrainRing.DbAdapter.Entities;
using BrainRing.Domain.Entities;

namespace BrainRing.DbAdapter.Mappers.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserEntity>().ReverseMap();
        }
    }
}
