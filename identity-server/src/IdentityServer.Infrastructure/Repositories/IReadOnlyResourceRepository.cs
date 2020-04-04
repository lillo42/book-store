using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Infrastructure.Entities;

namespace IdentityServer.Infrastructure.Repositories
{
    public interface IReadOnlyResourceRepository
    {
        Task<Resource> GetByNameAsync(string name);
        
        Task<IEnumerable<Resource>> GetByScopesAsync(IEnumerable<string> scope);
        
        Task<IEnumerable<Resource>> GetAllAsync();
    }
}