using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Raven.Client.Documents.Session;

namespace IdentityServer.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IAsyncDocumentSession _session;

        public EventRepository(IAsyncDocumentSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task CreateAsync(Event entity, CancellationToken cancellationToken = default)
        {
            await _session.StoreAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public Task UpdateAsync(Event entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(Event entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async Task SaveAsync(IEnumerable<Event> events, CancellationToken cancellationToken = default)
        {
            foreach (var @event in events)
            {
                await _session.StoreAsync(@event, cancellationToken)
                    .ConfigureAwait(false);   
            }
            
            await _session.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}