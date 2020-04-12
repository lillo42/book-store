using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IUserRepository : IRepository<User>, IReadOnlyUserRepository
    {
        Task AddPermissionsAsync(User entity, CancellationToken cancellationToken = default);
        
        Task RemovePermissionsAsync(User entity, CancellationToken cancellationToken = default);
        
        Task AddRolesAsync(User entity, CancellationToken cancellationToken = default);
        
        Task RemoveRolesAsync(User entity, CancellationToken cancellationToken = default);
    }
}