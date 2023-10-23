using Microsoft.EntityFrameworkCore.Storage;

namespace Ordering.Domain.Prematives;

public interface IUnitOfWork : IDisposable
{
    bool HasActiveTransaction { get; }
    IDbContextTransaction GetCurrentTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(IDbContextTransaction transaction);
    void RollbackTransaction();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    IExecutionStrategy CreateExecutionStrategy();
}
