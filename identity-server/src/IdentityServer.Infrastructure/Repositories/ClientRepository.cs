using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;

namespace IdentityServer.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        
        public async Task CreateAsync(Client entity, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            entity.Id = Guid.NewGuid();
            await connection.ExecuteAsync(
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
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
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

        #region Roles
        public async Task AddRoleAsync(Client entity, Role role, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "INSERT INTO public.\"ClientsRoles\" (client_id, role_id) VALUES (:client_id, :role_id)",
                    new { client_id = entity.Id, role_id = role.Id })
                .ConfigureAwait(false);
        }

        public async Task RemoveRoleAsync(Client entity, Role role, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsRoles\" WHERE client_id = :client_id AND role_id = :role_id",
                    new { client_id = entity.Id, role_id = role.Id })
                .ConfigureAwait(false);
        }
        #endregion

        #region Permssions
        public async Task AddPermissionAsync(Client entity, Permission permission, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "INSERT INTO public.\"ClientsPermissions\" (client_id, permission_id) VALUES (:client_id, :permission_id)",
                    new { client_id = entity.Id, permission_id = permission.Id })
                .ConfigureAwait(false);
        }

        public async Task RemovePermissionAsync(Client entity, Permission permission, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsPermissions\" WHERE client_id = :client_id AND  permission_id = :permission_id",
                    new { client_id = entity.Id, permission_id = permission.Id })
                .ConfigureAwait(false);
        }
        #endregion

        #region Resource

        public async Task AddResourceAsync(Client entity, Resource resource, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "INSERT INTO public.\"ClientsResources\" (client_id, resource_id) VALUES (:client_id, :resource_id)",
                    new { client_id = entity.Id, resource_id = resource.Id })
                .ConfigureAwait(false);
        }

        public async Task RemoveResourceAsync(Client entity, Resource resource, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                "DELETE FROM public.\"ClientsResources\" WHERE client_id = :client_id AND resource_id = :resource_id",
                new {client_id = entity.Id, resource_id = resource.Id});
        }

        #endregion
        
        public async Task DeleteAsync(Client entity, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsPermissions\" WHERE \"client_id\" = :client_id",
                    new {client_id = entity.Id})
                .ConfigureAwait(false);
            
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsResources\" WHERE \"client_id\" = :client_id",
                    new {client_id = entity.Id})
                .ConfigureAwait(false);
            
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsRoles\" WHERE \"client_id\" = :client_id",
                    new {client_id = entity.Id})
                .ConfigureAwait(false);
            
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"Clients\" WHERE \"id\" = :id",
                    new {id = entity.Id})
                .ConfigureAwait(false);
        }

        public async Task<Client> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            var multi = await connection.QueryMultipleAsync($@"
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
                    P.""name"" AS Name,
                    P.""display_name"" AS DisplayName,
                    P.""description"" AS Description
                FROM public.""Permissions"" P
                INNER JOIN public.""ClientsPermissions"" CP ON P.""id"" = CP.""permission_id""
                WHERE CP.""client_id"" = :id;
                SELECT
                    R.""id"" AS Id,
                    R.""name"" AS Name,
                    R.""display_name"" AS DisplayName,
                    R.""description"" AS Description
                FROM public.""Roles"" R
                INNER JOIN public.""ClientsRoles"" CR ON R.""id"" = CR.""role_id""
                WHERE CR.""client_id"" = :id;
                SELECT
                    R.""id"" AS Id,
                    R.""name"" AS Name,
                    R.""display_name"" AS DisplayName,
                    R.""description"" AS Description,
                    R.""is_active"" AS IsEnable
                FROM public.""Resources"" R
                INNER JOIN public.""ClientsResources"" CR ON R.""id"" = CR.""resource_id""
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
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            var client = await connection.QueryFirstOrDefaultAsync<Client>($@"
                SELECT 
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
            
            var multi = await connection.QueryMultipleAsync($@"
                SELECT
                    P.""id"" AS Id,
                    P.""name"" AS Name,
                    P.""display_name"" AS DisplayName,
                    P.""description"" AS Description
                FROM public.""Permissions"" P
                INNER JOIN public.""ClientsPermissions"" CP ON P.""id"" = CP.""permission_id""
                WHERE CP.""client_id"" = :id;
                SELECT
                    R.""id"" AS Id,
                    R.""name"" AS Name,
                    R.""display_name"" AS DisplayName,
                    R.""description"" AS Description
                FROM public.""Roles"" R
                INNER JOIN public.""ClientsRoles"" CR ON R.""id"" = CR.""role_id""
                WHERE CR.""client_id"" = :id;
                SELECT
                    R.""id"" AS Id,
                    R.""name"" AS Name,
                    R.""display_name"" AS DisplayName,
                    R.""description"" AS Description,
                    R.""is_active"" AS IsEnable
                FROM public.""Resources"" R
                INNER JOIN public.""ClientsResources"" CR ON R.""id"" = CR.""resource_id""
                WHERE CR.""client_id"" = :id;
                ", new {id =  client.Id})
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

        public async IAsyncEnumerable<Client> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await using var loader = await _unitOfWork.CreateUnsafeConnection(cancellationToken).ConfigureAwait(false);
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            var reader = await connection.ExecuteReaderAsync($@" 
                SELECT 
                    ""id"" AS Id,
                    ""name"" AS Name, 
                    ""is_active"" AS IsEnable, 
                    ""client_id"" AS ClientId, 
                    ""client_secret"" AS ClientSecret 
                FROM public.""Clients""")
                .ConfigureAwait(false);

            var parse = reader.GetRowParser<Client>();
            while (await reader.ReadAsync(cancellationToken)
                .ConfigureAwait(false))
            {
                var client = parse(reader);

                var multi = await loader.QueryMultipleAsync($@"
                SELECT
                    P.""id"" AS Id,
                    P.""name"" AS Name,
                    P.""display_name"" AS DisplayName,
                    P.""description"" AS Description
                FROM public.""Permissions"" P
                INNER JOIN public.""ClientsPermissions"" CP ON P.""id"" = CP.""permission_id""
                WHERE CP.""client_id"" = :id;
                SELECT
                    R.""id"" AS Id,
                    R.""name"" AS Name,
                    R.""display_name"" AS DisplayName,
                    R.""description"" AS Description
                FROM public.""Roles"" R
                INNER JOIN public.""ClientsRoles"" CR ON R.""id"" = CR.""role_id""
                WHERE CR.""client_id"" = :id;
                SELECT
                    R.""id"" AS Id,
                    R.""name"" AS Name,
                    R.""display_name"" AS DisplayName,
                    R.""description"" AS Description,
                    R.""is_active"" AS IsEnable
                FROM public.""Resources"" R
                INNER JOIN public.""ClientsResources"" CR ON R.""id"" = CR.""resource_id""
                WHERE CR.""client_id"" = :id", new {id =  client.Id})
                    .ConfigureAwait(false);


                var permissions = await multi.ReadAsync<Permission>().ConfigureAwait(false);
                var roles = await multi.ReadAsync<Role>().ConfigureAwait(false);
                var resources = await multi.ReadAsync<Resource>().ConfigureAwait(false);

                client.Permissions = permissions.ToHashSet();
                client.Roles = roles.ToHashSet();
                client.Resources = resources.ToHashSet();
                
                yield return client;
            }
        }

        public async Task<bool> ExistNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.ExecuteScalarAsync<bool>(
                    "SELECT TRUE FROM public.\"Clients\" where \"name\" = :name",
                    new {name})
                .ConfigureAwait(false);
        }

        public async Task<bool> ExistClientIdAsync(string clientId, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.ExecuteScalarAsync<bool>(
                    "SELECT TRUE FROM public.\"Clients\" where \"client_id\" = :client_id",
                    new {client_id = clientId})
                .ConfigureAwait(false);
        }
    }
}