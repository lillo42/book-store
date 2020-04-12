using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IResourceRepository : IRepository<Resource>, IReadOnlyResourceRepository
    {
    }
}