using System.Data.Common;

namespace IdentityServer.Infrastructure.Abstractions
{
    public interface IDbConnectionFactory
    {
        DbConnection Create();
    }
}