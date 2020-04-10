using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IEventRepository : IRepository<IEvent>
    {
        Task SaveAsync(IEnumerable<IEvent> @events, CancellationToken cancellationToken = default);
    }
}