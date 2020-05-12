using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Infrastructure.Abstractions
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        ValueTask<DbConnection> GetOrCreateDbConnection(CancellationToken cancellationToken = default);
        Task<DbConnection> CreateUnsafeConnection(CancellationToken cancellationToken = default);
        Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        Task CommitAsync(CancellationToken cancellationToken);
    }
}