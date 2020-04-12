using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IReadOnlyRoleRepository
    {
        Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default);
    }
}