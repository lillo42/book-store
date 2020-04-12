using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;

namespace IdentityServer.Infrastructure.Abstractions.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        Task SaveAsync(IEnumerable<Event> @events, CancellationToken cancellationToken = default);
    }
}