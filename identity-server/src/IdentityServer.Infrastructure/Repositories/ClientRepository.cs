using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Npgsql;

namespace IdentityServer.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly NpgsqlConnection _connection;

        public ClientRepository(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }
        
        public async Task CreateAsync(Client entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Guid.NewGuid();
            await _connection.ExecuteAsync(
                    "INSERT INTO public.\"Clients\" (\"id\", \"name\", \"is_active\", \"client_id\", \"client_secret\") VALUES (:id, :name, :is_active, :client_id, :client_secret)",
                    new
                    {
                        id = entity.Id, 
                        name = entity.Name, 
                        is_active = entity.IsEnable, 
                        client_id = entity.ClientId,
                        client_secret = entity.ClientSecret
                    })
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(Client entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "UPDATE public.\"Clients\" SET \"name\" = :name, \"is_active\" = :is_active, \"client_id\" = :client_id, \"client_secret\" = :client_secret WHERE \"id\" = :id",
                    new
                    {
                        id = entity.Id, 
                        name = entity.Name, 
                        is_active = entity.IsEnable, 
                        client_id = entity.ClientId,
                        client_secret = entity.ClientSecret
                    })
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(Client entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsPermissions\" WHERE \"client_id\" = :client_id",
                    new {client_id = entity.Id})
                .ConfigureAwait(false);
            
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsResources\" WHERE \"client_id\" = :client_id",
                    new {client_id = entity.Id})
                .ConfigureAwait(false);
            
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsRoles\" WHERE \"client_id\" = :client_id",
                    new {client_id = entity.Id})
                .ConfigureAwait(false);
            
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"Clients\" WHERE \"id\" = :id",
                    new {id = entity.Id})
                .ConfigureAwait(false);
        }

        public async Task<Client> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var multi = await _connection.QueryMultipleAsync($@"
                SELECT 
                    ""id"" AS Id,
                    ""name"" AS Name, 
                    ""is_active"" AS IsEnable, 
                    ""client_id"" AS ClientId, 
                    ""client_secret"" AS ClientSecret 
                FROM public.""Clients"" 
                WHERE  ""id"" = :id
                LIMIT 1;
                SELECT
                    P.""id"" AS Id,
                    P.""name"" AS Name
                FROM public.""Permissions"" P
                INNER JOIN public.""ClientsPermissions"" CP ON P.""id"" = CP.""permission_id""
                WHERE CP.""client_id"" = :id;
                SELECT
                    R.""id"" AS Id,
                    R.""name"" AS Name
                FROM public.""Roles"" R
                INNER JOIN public.""ClientsRoles"" CR ON R.""id"" = CP.""role_id""
                WHERE CR.""client_id"" = :id;
                SELECT
                    R.""id"" AS Id,
                    R.""name"" AS Name
                FROM public.""Resources"" R
                INNER JOIN public.""ClientsResources"" CR ON R.""id"" = CP.""resource_id""
                WHERE CR.""client_id"" = :id;", new {id})
                .ConfigureAwait(false);

            var client = await multi.ReadFirstOrDefaultAsync<Client>()
                .ConfigureAwait(false);

            if (client == null)
            {
                return null;
            }

            var permissions = await multi.ReadAsync<Permission>()
                .ConfigureAwait(false);
            
            var roles = await multi.ReadAsync<Role>()
                .ConfigureAwait(false);
            
            var resource = await multi.ReadAsync<Resource>()
                .ConfigureAwait(false);
            
            client.Permissions = permissions.ToHashSet();
            client.Roles = roles.ToHashSet();
            client.Resources = resource.ToHashSet();
            
            return client;
        }

        public async Task<Client> GetByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
        {
            var client = await _connection.QueryFirstOrDefaultAsync<Client>($@"SELECT 
                    ""id"" AS Id,
                    ""name"" AS Name, 
                    ""is_active"" AS IsEnable, 
                    ""client_id"" AS ClientId, 
                    ""client_secret"" AS ClientSecret 
                FROM public.""Clients"" 
                WHERE  ""client_id"" = :client_id
                LIMIT 1", new {client_id =  clientId})
                .ConfigureAwait(false);

            if (client == null)
            {
                return null;
            }
            
            var multi = await _connection.QueryMultipleAsync($@"
                SELECT
                    P.""id"" AS Id,
                    P.""name"" AS Name
                FROM public.""Permissions"" P
                INNER JOIN public.""ClientsPermissions"" CP ON P.""id"" = CP.""permission_id""
                WHERE CP.""client_id"" = :id;
                SELECT
                    R.""id"" AS Id,
                    R.""name"" AS Name
                FROM public.""Roles"" R
                INNER JOIN public.""ClientsRoles"" CR ON R.""id"" = CP.""role_id""
                WHERE CR.""client_id"" = :id;
                SELECT
                    R.""id"" AS Id,
                    R.""name"" AS Name
                FROM public.""Resources"" R
                INNER JOIN public.""ClientsResources"" CR ON R.""id"" = CP.""resource_id""
                WHERE CR.""client_id"" = :id;", new {id =  client})
                .ConfigureAwait(false);
            
            var permissions = await multi.ReadAsync<Permission>()
                .ConfigureAwait(false);
            
            var roles = await multi.ReadAsync<Role>()
                .ConfigureAwait(false);
            
            var resource = await multi.ReadAsync<Resource>()
                .ConfigureAwait(false);
            
            client.Permissions = permissions.ToHashSet();
            client.Roles = roles.ToHashSet();
            client.Resources = resource.ToHashSet();
            
            return client;
        }
    }
}