using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IClientRepository : IRepository<Client>, IReadOnlyClientRepository
    {
        Task AddRoleAsync(Client entity, Role role, CancellationToken cancellation = default);
        Task RemoveRoleAsync(Client entity, Role role, CancellationToken cancellation = default);
        
        Task AddPermissionAsync(Client entity, Permission permission, CancellationToken cancellation = default);
        Task RemovePermissionAsync(Client entity, Permission permission, CancellationToken cancellation = default);
        
        Task AddResourceAsync(Client entity, Resource resource, CancellationToken cancellation = default);
        Task RemoveResourceAsync(Client entity, Resource resource, CancellationToken cancellation = default);
    }
}