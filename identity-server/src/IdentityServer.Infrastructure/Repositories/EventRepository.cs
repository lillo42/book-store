using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;

namespace IdentityServer.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        public Task CreateAsync(IEvent entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(IEvent entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(IEvent entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}