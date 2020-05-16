using System.Linq;
using IdentityServer4.Models;
using Raven.Client.Documents.Indexes;

namespace IdentityServer.Web.IdentityServer4.Index
{
    public class PersistedGrant_BySubjectIdByClientIdByType : AbstractIndexCreationTask<PersistedGrant>
    {
        public PersistedGrant_BySubjectIdByClientIdByType()
        {
            Map = persistedGrant => from grant in persistedGrant
                select new
                {
                    SubjectId = grant.SubjectId,
                    ClientId = grant.ClientId,
                    Type = grant.Type
                };
        }
    }
}