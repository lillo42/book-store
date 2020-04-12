using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;

namespace IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly
{
    public interface IReadOnlyResourceRepository
    {
        Task<Resource> GetByIdAsync(Guid id, CancellationToken cancellation = default);
        Task<Resource> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Resource>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Resource>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);
    }
}