using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;

namespace IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly
{
    public interface IReadOnlyClientRepository
    {
        Task<Client> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Client> GetByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
    }
}