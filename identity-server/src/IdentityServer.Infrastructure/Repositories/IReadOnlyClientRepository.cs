using System.Threading.Tasks;
using IdentityServer.Infrastructure.Entities;

namespace IdentityServer.Infrastructure.Repositories
{
    public interface IReadOnlyClientRepository
    {
        Task<Client> GetClientAsync(string clientId);
    }
}