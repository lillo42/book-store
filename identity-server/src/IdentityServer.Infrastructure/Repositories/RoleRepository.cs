using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Npgsql;

namespace IdentityServer.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DbConnection _connection;

        public RoleRepository(DbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }
        
        public async Task CreateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Guid.NewGuid();
            await _connection.ExecuteAsync(
                    "INSERT INTO public.\"Roles\" (\"id\", \"name\", \"display_name\", \"description\") VALUES (:id, :name,  :display_name, :description)", 
                    new { id = entity.Id, name = entity.Name,  display_name = entity.DisplayName, description = entity.Description })
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "UPDATE public.\"Roles\" SET  \"name\" = :name, \"display_name\" = :display_name, \"description\" = :description WHERE \"id\" = :id",
                    new { id = entity.Id, name = entity.Name,  display_name = entity.DisplayName, description = entity.Description })
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(Role entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"RolesPermissions\" WHERE \"role_id\" = :role_id", new {role_id = entity.Id})
                .ConfigureAwait(false);
            
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"Roles\" WHERE \"id\" = :id", new {id = entity.Id})
                .ConfigureAwait(false);
        }

        public async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var multi = await _connection.QueryMultipleAsync($@"
            SELECT
                R.""id"" AS Id,
                R.""name"" AS Name,
                R.""display_name"" AS DisplayName,
                R.""description"" AS Description
            FROM public.""Roles"" R
            WHERE  R.""id"" = :id
            LIMIT 1;
            SELECT
                P.""id"" As Id,
                P.""name"" AS ""Name"",
                P.""display_name"" AS DisplayName,
                P.""description"" AS Description
            FROM public.""RolesPermissions"" RP
            INNER JOIN public.""Permissions"" P ON  P.""id"" = RP.""permission_id""
            WHERE RP.""role_id"" = :id;",
                    new {id})
            .ConfigureAwait(false);

            var role = await multi.ReadFirstOrDefaultAsync<Role>()
                .ConfigureAwait(false);

            var permissions = await multi.ReadAsync<Permission>()
                .ConfigureAwait(false);
            
            role.Permissions = permissions.ToHashSet();
            
            return role;
        }
        
        public async Task AddPermissionAsync(Role entity, Permission permission, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "INSERT INTO public.\"RolesPermissions\" (\"role_id\", \"permission_id\") VALUES (:role_id, :permission_id);",
                    new { role_id = entity.Id, permission_id = permission.Id })
                .ConfigureAwait(false);   
        }

        public async Task RemovePermissionAsync(Role entity, Permission permission, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"RolesPermissions\" WHERE \"role_id\" = :role_id AND  \"permission_id\" = :permission_id",
                    new { role_id = entity.Id, permission_id = permission.Id})
                .ConfigureAwait(false);
        }

        public async Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default) 
            => await _connection.ExecuteScalarAsync<bool>(
                    "SELECT TRUE FROM public.\"Roles\" where \"id\" = :id",
                    new {id})
                .ConfigureAwait(false);

        public async IAsyncEnumerable<Role> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var reader = await _connection.ExecuteReaderAsync($@"
            SELECT
                R.""id"" AS Id,
                R.""name"" AS Name,
                R.""display_name"" AS DisplayName,
                R.""description"" AS Description
            FROM public.""Roles"" R");
            
            var parse = reader.GetRowParser<Role>();
            while (await reader.ReadAsync(cancellationToken))
            {
                var role = parse(reader);
                var permissions = await _connection.QueryAsync<Permission>($@"
                SELECT
                    P.""id"" As Id,
                    P.""name"" AS ""Name"",
                    P.""display_name"" AS DisplayName,
                    P.""description"" AS Description
                FROM public.""RolesPermissions"" RP
                INNER JOIN public.""Permissions"" P ON  P.""id"" = RP.""permission_id""
                WHERE RP.""role_id"" = :id", new { id = role.Id});

                foreach (var permission in permissions)
                {
                    role.Permissions.Add(permission);
                }
                
                yield return role;
            }
        }
    }
}