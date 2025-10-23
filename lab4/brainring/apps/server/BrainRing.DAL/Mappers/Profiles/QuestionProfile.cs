using AutoMapper;
using BrainRing.DbAdapter.Entities;
using BrainRing.Domain.Entities;

namespace BrainRing.DbAdapter.Mappers.Profiles
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<Question, QuestionEntity>().ReverseMap();
        }
    }
}
