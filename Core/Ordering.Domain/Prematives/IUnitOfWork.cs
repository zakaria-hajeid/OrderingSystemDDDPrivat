using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace Ordering.Domain.Prematives;

public interface IUnitOfWork : IDisposable
{
    bool HasActiveTransaction { get; }
    IDbContextTransaction GetCurrentTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(IDbContextTransaction transaction);
    void RollbackTransaction();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task PublishEventAsyncAsync(CancellationToken cancellationToken = default(CancellationToken));
    IExecutionStrategy CreateExecutionStrategy();
    DbConnection DbConnection();
     


}
