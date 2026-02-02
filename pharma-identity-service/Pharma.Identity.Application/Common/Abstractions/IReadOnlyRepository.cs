using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Pharma.Identity.Application.Common.Models;

namespace Pharma.Identity.Application.Common.Abstractions;

public interface IReadOnlyRepository<TEntity> where TEntity : class
{
    Task<List<TResult>> FindAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default
    );

    Task<List<TResult>> FindAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );

    Task<Pagination<TResult>> FindAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        Expression<Func<TEntity, TResult>> selector,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? include = null,
        Expression<Func<TEntity, TEntity>>? selector = null,
        CancellationToken cancellationToken = default
    );

    IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate);

    IQueryable<TEntity> GetQueryable();
}