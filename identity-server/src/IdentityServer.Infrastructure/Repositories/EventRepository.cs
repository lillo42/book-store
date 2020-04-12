using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories;

namespace IdentityServer.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        public Task CreateAsync(Event entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(Event entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(Event entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveAsync(IEnumerable<Event> events, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}