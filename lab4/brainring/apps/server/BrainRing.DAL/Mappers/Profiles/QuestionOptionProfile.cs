using AutoMapper;
using BrainRing.DbAdapter.Entities;
using BrainRing.Domain.Entities;

namespace BrainRing.DbAdapter.Mappers.Profiles
{
    public class QuestionOptionProfile : Profile
    {
        public QuestionOptionProfile()
        {
            CreateMap<QuestionOption, QuestionOptionEntity>().ReverseMap();
        }
    }
}
