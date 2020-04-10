using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Infrastructure.Abstractions.Repositories;

namespace IdentityServer.Infrastructure.Abstractions
{
    public interface IUnitOfWork<T>
        where T : IRepository
    {
        
        T Repository { get; }
        
        IDisposable BeginTransaction();
        Task SaveAsync(CancellationToken cancellationToken);
    }
}