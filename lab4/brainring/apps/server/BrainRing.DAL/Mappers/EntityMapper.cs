using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BrainRing.DbAdapter.Interfaces.Mappers;
using System.Linq.Expressions;

namespace BrainRing.DbAdapter.Mappers
{
    public class EntityMapper : IEntityMapper
    {
        private readonly IMapper _mapper;
        public EntityMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TDestination Map<TSource, TDestination>(TSource source) =>
            _mapper.Map<TDestination>(source);

        public void Map<TSource, TDestination>(TSource source, TDestination destination) =>
            _mapper.Map(source, destination);

        public Expression<Func<TDestination, TResult>> MapExpression<TSource, TDestination, TResult>(Expression<Func<TSource, TResult>> expression) =>
            _mapper.MapExpression<Expression<Func<TDestination, TResult>>>(expression);

    }
}
