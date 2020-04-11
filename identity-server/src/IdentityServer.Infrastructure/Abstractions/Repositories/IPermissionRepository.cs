using IdentityServer.Domain.Common;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IPermissionRepository : IRepository<Permission>, IReadOnlyPermissionRepository
    {
        
    }
}