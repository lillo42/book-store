using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;

namespace IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly
{
    public interface IReadOnlyPermissionRepository
    {
        Task<Permission> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
        IAsyncEnumerable<Permission> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
        
        Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default);
        
        
        Task<bool> ExistAsync(string permissionName, CancellationToken cancellationToken = default);
    }
}