using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Application.Common.Models;
using Pharma.Identity.Infrastructure.Persistence;

namespace Pharma.Identity.Infrastructure.Repositories;

public class ReadOnlyRepository<TEntity>(ReadOnlyDbContext dbContext)
    : IReadOnlyRepository<TEntity> where TEntity : class
{
    private IQueryable<TEntity> DbSet => dbContext.Set<TEntity>();

    public async Task<List<TResult>> FindAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyQueryOptions(DbSet, predicate, include, orderBy);

        return await query.Select(selector).ToListAsync(cancellationToken);
    }

    public async Task<List<TResult>> FindAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default
    )
    {
        return await FindAsync(predicate, null, null, selector, cancellationToken);
    }

    public async Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyQueryOptions(DbSet, predicate, include, orderBy);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        CancellationToken cancellationToken = default
    )
    {
        return await FindAsync(predicate, null, orderBy, cancellationToken);
    }

    public async Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return await FindAsync(predicate, null, null, cancellationToken);
    }

    public async Task<Pagination<TResult>> FindAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy, Expression<Func<TEntity, TResult>> selector,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var offset = (pageNumber - 1) * pageSize;

        var baseQuery = ApplyQueryOptions(DbSet, predicate, include, orderBy);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var pagedQuery = baseQuery.Skip(offset).Take(pageSize);

        var data = await pagedQuery.Select(selector).ToListAsync(cancellationToken);

        return new Pagination<TResult>(data, totalCount, pageSize, pageNumber);
    }

    public async Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        Expression<Func<TEntity, TEntity>>? selector = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyQueryOptions(DbSet, predicate, include);

        if (selector != null)
        {
            query = query.Select(selector);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return DbSet;
    }

    private static IQueryable<TEntity> ApplyQueryOptions(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (include != null)
        {
            query = include(query);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query.AsNoTracking();
    }
}