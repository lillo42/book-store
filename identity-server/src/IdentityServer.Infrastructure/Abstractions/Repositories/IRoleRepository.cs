using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IRoleRepository : IRepository<Role>, IReadOnlyRoleRepository
    {
        Task AddPermissionAsync(Role entity, Permission permission, CancellationToken cancellationToken = default);
        
        Task RemovePermissionAsync(Role entity, Permission permission, CancellationToken cancellationToken = default);
    }
}