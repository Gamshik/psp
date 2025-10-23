using System.Linq.Expressions;

namespace BrainRing.DbAdapter.Interfaces.Mappers
{
    public interface IEntityMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);
        void Map<TSource, TDestination>(TSource source, TDestination destination);
        Expression<Func<TDestination, TResult>> MapExpression<TSource, TDestination, TResult>(Expression<Func<TSource, TResult>> expression);
    }
}
