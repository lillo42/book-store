using System.Linq;
using IdentityServer4.Models;
using Raven.Client.Documents.Indexes;

namespace IdentityServer.Web.IdentityServer4.Index
{
    public class PersistedGrant_ByKey : AbstractIndexCreationTask<PersistedGrant>
    {
        public PersistedGrant_ByKey()
        {
            Map = persistedGrant => from grant in persistedGrant
                select new
                {
                    Key = grant.Key
                };
        }
    }
}