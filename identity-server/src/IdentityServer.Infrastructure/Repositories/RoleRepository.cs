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
    public class RoleRepository : IRoleRepository
    {
        private readonly NpgsqlConnection _connection;

        public RoleRepository(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }


        public async Task CreateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Guid.NewGuid();
            await _connection.ExecuteAsync(
                    "INSERT INTO public.\"Roles\" (\"id\", \"name\", \"display_name\", \"description\") VALUES (@id, @name,  @display_name, @description)", 
                    new { id = entity.Id, name = entity.Name,  display_name = entity.DisplayName, description = entity.Description })
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "UPDATE public.\"Roles\" SET  \"name\" = @name, \"display_name\" = @display_name, \"description\" = @description WHERE \"id\" = @id",
                    new { id = entity.Id, name = entity.Name,  display_name = entity.DisplayName, description = entity.Description })
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(Role entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"RolesPermissions\" WHERE \"role_id\" = @role_id", new {role_id = entity.Id})
                .ConfigureAwait(false);
            
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"Roles\" WHERE \"id\" = @id", new {id = entity.Id})
                .ConfigureAwait(false);
        }

        public async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var multi = await _connection.QueryMultipleAsync($@"
                SELECT 
                    ""id"" AS Id,
                    ""name"" AS Name, 
                    ""display_name"" AS DisplayName, 
                    ""description"" AS Description 
                FROM public.""Roles"" 
                WHERE  ""id"" = @id;
                SELECT
                    ""id"" AS Id,
                    ""name"" AS Name, 
                    ""display_name"" AS DisplayName, 
                    ""description"" AS Description
                FROM public.""Permissions"" P
                INNER JOIN public.""RolesPermissions"" RP ON P.""id"" = RP.""permission_id""
                WHERE RP.""role_id"" = @id;", new {id})
            .ConfigureAwait(false);

            var role = await multi.ReadFirstOrDefaultAsync<Role>()
                .ConfigureAwait(false);

            var permissions = await multi.ReadAsync<Permission>()
                .ConfigureAwait(false);
            
            role.Permissions = permissions.ToHashSet();
            
            return role;
        }

        public async Task AddPermissionsAsync(Role entity, CancellationToken cancellationToken = default)
        {
            foreach (var permission in entity.Permissions ?? Enumerable.Empty<Permission>())
            {
                await _connection.ExecuteAsync(
                        "INSERT INTO public.\"RolesPermissions\" (\"role_id\", \"permission_id\") VALUES (@role_id, @permission_id) ON CONFLICT DO NOTHING;",
                        new { role_id = entity.Id, permission_id = permission.Id })
                    .ConfigureAwait(false);   
            }
        }

        public async Task RemovePermissionsAsync(Role entity, CancellationToken cancellationToken = default)
        {
            if (entity.Permissions.Count == 0)
            {
                return;
            }

            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"RolesPermissions\" WHERE \"role_id\" = @role_id AND  \"permission_id\" NOT IN @permisions",
                    new { role_id = entity.Id, permisions = entity.Permissions.Select(x=> x.Id).ToArray()})
                .ConfigureAwait(false);
        }
    }
}