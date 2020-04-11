using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IReadOnlyPermissionRepository
    {
        Task<Permission> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}