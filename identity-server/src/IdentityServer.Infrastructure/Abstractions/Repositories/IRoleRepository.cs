using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IRoleRepository : IRepository<Role>, IReadOnlyRoleRepository
    {
        Task AddPermissionsAsync(Role entity, CancellationToken cancellationToken = default);
        
        Task RemovePermissionsAsync(Role entity, CancellationToken cancellationToken = default);
    }
}