using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Web.IdentityServer4.Index;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;

namespace IdentityServer.Web.IdentityServer4.Store
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IDocumentStore _store;

        public PersistedGrantStore(IDocumentStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            using var session = _store.OpenAsyncSession();
            await session.StoreAsync(grant).ConfigureAwait(false);
            await session.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            using var session = _store.OpenAsyncSession(new SessionOptions
            {
                NoTracking = true
            });
            
            return await session.Query<PersistedGrant, PersistedGrant_ByKey>()
                .FirstOrDefaultAsync(x => x.Key == key);
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            using var session = _store.OpenAsyncSession(new SessionOptions
            {
                NoTracking = true
            });
            
            return await session.Query<PersistedGrant, PersistedGrant_BySubjectIdByClientIdByType>()
                .Where(x => x.SubjectId == subjectId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task RemoveAsync(string key)
        {
            await _store.Operations.SendAsync(new DeleteByQueryOperation<PersistedGrant, PersistedGrant_ByKey>(x => x.Key == key))
                .ConfigureAwait(false);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await _store.Operations.SendAsync(new DeleteByQueryOperation<PersistedGrant, PersistedGrant_BySubjectIdByClientIdByType>(
                    x => x.SubjectId == subjectId && x.ClientId == clientId ))
                .ConfigureAwait(false);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await _store.Operations.SendAsync(new DeleteByQueryOperation<PersistedGrant, PersistedGrant_BySubjectIdByClientIdByType>(
                x => x.SubjectId == subjectId && x.ClientId == clientId && x.Type == type))
            .ConfigureAwait(false);
        }
    }
}