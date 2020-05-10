using System.Data.Common;

namespace IdentityServer.Infrastructure
{
    public interface IDbFactory
    {
        DbConnection Create();
    }
}