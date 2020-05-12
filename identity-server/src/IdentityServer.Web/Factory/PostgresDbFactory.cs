using System;
using System.Data.Common;
using IdentityServer.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Npgsql.Logging;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace IdentityServer.Web.Factory
{
    public class PostgresDbFactory : IDbFactory
    {
        private readonly IConfiguration _configuration;

        public PostgresDbFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Debug, true, true);
        }

        public DbConnection Create()
        {
            return new ProfiledDbConnection(
                new NpgsqlConnection(_configuration.GetConnectionString("Postgres")), 
                MiniProfiler.Current);
        }
    }
}