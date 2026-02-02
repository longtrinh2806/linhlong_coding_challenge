using Microsoft.EntityFrameworkCore.Storage;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Infrastructure.Repositories;

namespace Pharma.Identity.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;
    private readonly Dictionary<Type, object> _repositories = new();
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);

        if (_repositories.TryGetValue(type, out var value))
        {
            return (IGenericRepository<TEntity>)value;
        }

        var repositoryType = typeof(GenericRepository<>).MakeGenericType(type);
        var repositoryInstance = Activator.CreateInstance(repositoryType, this);

        value = repositoryInstance!;

        _repositories[type] = value;

        return (IGenericRepository<TEntity>)value;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}