using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Pharma.Identity.Application.Common.Abstractions;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        CancellationToken cancellationToken = default);

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task Delete(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}