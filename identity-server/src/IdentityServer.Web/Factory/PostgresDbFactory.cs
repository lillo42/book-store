using System;
using System.Data.Common;
using IdentityServer.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql;
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
        }

        public DbConnection Create()
        {
            var connection = new ProfiledDbConnection(new NpgsqlConnection(_configuration.GetConnectionString("Postgres")), MiniProfiler.Current);

            connection.Open();

            return connection;
        }
    }
}