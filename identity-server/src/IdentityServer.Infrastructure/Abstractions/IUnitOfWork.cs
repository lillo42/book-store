using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Infrastructure.Abstractions.Repositories;

namespace IdentityServer.Infrastructure.Abstractions
{
    public interface IUnitOfWork
    {
        IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        Task CommitAsync(CancellationToken cancellationToken);
        
        IClientRepository ClientRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IResourceRepository ResourceRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository { get; }
    }
}