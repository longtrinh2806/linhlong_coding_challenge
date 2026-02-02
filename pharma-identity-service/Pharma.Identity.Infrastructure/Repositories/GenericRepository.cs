using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Infrastructure.Persistence;

namespace Pharma.Identity.Infrastructure.Repositories;

public class GenericRepository<TEntity>(
    IUnitOfWork unitOfWork,
    ApplicationDbContext dbContext
) : IGenericRepository<TEntity> where TEntity : class
{
    private DbSet<TEntity> DbSet => dbContext.Set<TEntity>();

    public async Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyQueryOptions(DbSet, predicate, include, orderBy);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await FindAsync(predicate, null, null, cancellationToken);
    }

    public Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var query = ApplyQueryOptions(DbSet, predicate, null, null);

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task UpdateRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);
        await unitOfWork.CommitAsync( cancellationToken);
    }

    public async Task Delete(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        await  unitOfWork.CommitAsync(cancellationToken);
    }

    private static IQueryable<TEntity> ApplyQueryOptions(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy)
    {
        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query.AsTracking();
    }
}