using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Infrastructure.Entities;
using Npgsql;

namespace IdentityServer.Infrastructure.Repositories
{
    public class ResourceRepository : IReadOnlyResourceRepository
    {
        private readonly string _connectionString;
        public ResourceRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<Resource> GetByNameAsync(string name)
        {
            await using var connection =  new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Resource>($@"
            SELECT 
                ""id"" AS Id,
                ""name"" AS Name,
                ""display_name"" AS DisplayName,
                ""description"" AS Description
            FROM public.""Resources""")
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Resource>> GetByScopesAsync(IEnumerable<string> scope)
        {
            await using var connection =  new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Resource>($@"
            SELECT 
                ""id"" AS Id,
                ""name"" AS Name,
                ""display_name"" AS DisplayName,
                ""description"" AS Description
            FROM public.""Resources""
            WHERE ""name"" IN @names", new { names = scope.ToArray() })
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Resource>> GetAllAsync()
        {
            await using var connection =  new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Resource>($@"
            SELECT 
                ""id"" AS Id,
                ""name"" AS Name,
                ""display_name"" AS DisplayName,
                ""description"" AS Description
            FROM public.""Resources""")
                .ConfigureAwait(false);
        }
    }
}