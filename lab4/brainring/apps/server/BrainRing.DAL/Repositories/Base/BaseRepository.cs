using BrainRing.Application.Interfaces.Repositories.Base;
using BrainRing.DbAdapter.Interfaces.Entities;
using BrainRing.DbAdapter.Interfaces.Mappers;
using BrainRing.Domain.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BrainRing.DbAdapter.Repositories.Base
{
    public class BaseRepository<T, Db> : IBaseRepository<T> where T : class, IBase 
        where Db : class, IBaseEntity
    {
        protected readonly BrainRingDbContext _context;
        protected readonly IEntityMapper _mapper;

        public BaseRepository(BrainRingDbContext context, IEntityMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
       
        public virtual IEnumerable<T> FindAll(bool isTracking = false, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<Db> entitiesQuery = _context.Set<Db>();

            if (!isTracking)
                entitiesQuery = entitiesQuery.AsNoTracking();

            var dbEntities = _applyIncludes(entitiesQuery, includes).AsEnumerable();

            return _mapper.Map<IEnumerable<Db>, IEnumerable<T>>(dbEntities);
        }

        public virtual async Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default, bool isTracking = false, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<Db> entitiesQuery = _context.Set<Db>();

            if (!isTracking)
                entitiesQuery = entitiesQuery.AsNoTracking();

            entitiesQuery = _applyIncludes(entitiesQuery, includes);

            var entity = await entitiesQuery.FirstOrDefaultAsync((entity) => entity.Id == id);

            if (entity == null) return null;

            return _mapper.Map<Db, T>(entity);
        }

        public virtual IEnumerable<T> FindByCondition(Expression<Func<T, bool>> condition, bool isTracking = false, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<Db> entityQuery = _context.Set<Db>();

            if (!isTracking)
                entityQuery = entityQuery.AsNoTracking();

            entityQuery = _applyIncludes(entityQuery, includes);

            var dbCondition = _mapper.MapExpression<T, Db, bool>(condition);

            var dbEntities = entityQuery.Where(dbCondition).AsEnumerable();

            return _mapper.Map<IEnumerable<Db>, IEnumerable<T>>(dbEntities);
        }

        public virtual async Task<T?> FindOneByConditionAsync(
            Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken = default,
            bool isTracking = false,
            params Expression<Func<T, object>>[] includes
            )
        {
            IQueryable<Db> entityQuery = _context.Set<Db>();

            if (!isTracking)
                entityQuery = entityQuery.AsNoTracking();

            entityQuery = _applyIncludes(entityQuery, includes);

            var dbCondition = _mapper.MapExpression<T, Db, bool>(condition);

            var dbEntity = await entityQuery.FirstOrDefaultAsync(dbCondition, cancellationToken);

            return _mapper.Map<Db, T>(dbEntity);
        }
        public virtual async Task<T?> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var dbEntity = _mapper.Map<T, Db>(entity);
                await _context
                    .Set<Db>()
                    .AddAsync(dbEntity, cancellationToken);

                await SaveChangesAsync(cancellationToken);

                _mapper.Map(dbEntity, entity);

                return entity;
            } 
            catch (DbUpdateException ex)
            {
                return null;
            }
        }

        public virtual async Task<T?> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingDbEntity = await _context.Set<Db>()
                    .FirstOrDefaultAsync(e => e.Id == entity.Id, cancellationToken);

                if (existingDbEntity == null)
                    return null;

                _mapper.Map(entity, existingDbEntity);

                await SaveChangesAsync(cancellationToken);

                return entity;
            }
            catch (DbUpdateException ex)
            {
                return null;
            }
        }

        public virtual async Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                var dbEntities = _mapper.Map<IEnumerable<T>, IEnumerable<Db>>(entities);
                _context.Set<Db>().UpdateRange(dbEntities);

                return await SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                return 0;
            }
        }

        public virtual async Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingDbEntity = await _context.Set<Db>()
                    .FirstOrDefaultAsync(e => e.Id == entity.Id, cancellationToken);

                if (existingDbEntity == null)
                    return false;

                _context.Set<Db>().Remove(existingDbEntity);

                await SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (DbUpdateException ex)
            {
                return false;
            }
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await _context
                .SaveChangesAsync(cancellationToken);

        protected IQueryable<Db> _applyIncludes(IQueryable<Db> query, Expression<Func<T, object>>[] includes)
        {

            for (var i = 0; i < includes.Length; i++)
            {
                var currInclude = includes[i];
                var member = currInclude.Body as MemberExpression
                             ?? ((UnaryExpression)currInclude.Body).Operand as MemberExpression;

                if (member != null)
                {
                    var propertyName = member.Member.Name;
                    query = query.Include(propertyName);
                }
            }

            return query;
        }
    }
}
