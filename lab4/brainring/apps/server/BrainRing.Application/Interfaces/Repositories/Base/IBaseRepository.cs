using BrainRing.Domain.Interfaces.Base;
using System.Linq.Expressions;

namespace BrainRing.Application.Interfaces.Repositories.Base
{
    public interface IBaseRepository<T> where T : class, IBase
    {
        IEnumerable<T> FindAll(bool isTracking = false, params Expression<Func<T, object>>[] includes);
        Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default, bool isTracking = false, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> condition, bool isTracking = false, params Expression<Func<T, object>>[] includes);
        Task<T?> FindOneByConditionAsync(
            Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken = default,
            bool isTracking = false,
            params Expression<Func<T, object>>[] includes
            );
        Task<T?> CreateAsync(T entity, CancellationToken cancellationToken = default);
        Task<T?> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
