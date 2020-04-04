using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Infrastructure.Entities;
using Npgsql;

namespace IdentityServer.Infrastructure.Repositories
{
    public class ClientRepository : IReadOnlyClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<Client> GetClientAsync(string clientId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection
                    .QueryFirstOrDefaultAsync<Client>($@"
                          SELECT 
                                ""id"" as Id, 
                                ""client_id"" as ClientId, 
                                ""password"" as Password 
                            FROM public.""Clients""
                            WHERE client_id = @clientId", new { clientId })
                    .ConfigureAwait(false);
            }
        }
    }
}