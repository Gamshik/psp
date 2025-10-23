using AutoMapper;
using BrainRing.DbAdapter.Entities;
using BrainRing.Domain.Entities;

namespace BrainRing.DbAdapter.Mappers.Profiles
{
    public class AnswerProfile : Profile
    {
        public AnswerProfile()
        {
            CreateMap<Answer, AnswerEntity>().ReverseMap();
        }
    }
}
