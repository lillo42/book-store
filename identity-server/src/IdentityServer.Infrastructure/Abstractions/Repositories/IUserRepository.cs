using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IUserRepository : IRepository<User>, IReadOnlyUserRepository
    {
        Task AddPermissionAsync(User entity, Permission permission, CancellationToken cancellationToken = default);
        
        Task RemovePermissionAsync(User entity, Permission permission,  CancellationToken cancellationToken = default);
        
        Task AddRoleAsync(User entity, Role role, CancellationToken cancellationToken = default);
        
        Task RemoveRoleAsync(User entity, Role role, CancellationToken cancellationToken = default);
    }
}