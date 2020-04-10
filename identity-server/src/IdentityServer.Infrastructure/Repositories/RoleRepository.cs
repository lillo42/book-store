using System;
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
                    "DELETE FROM public.\"Roles\" WHERE \"id\" = @id", new {id = entity.Id})
                .ConfigureAwait(false);
        }

        public Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            
            throw new NotImplementedException();
        }

        public async Task AddPermissionsAsync(Role entity, Permission permission, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "INSERT INTO public.\"RolesPermissions\" (\"role_id\", \"permission_id\") VALUES (@role_id, @permission_id)",
                    new { })
                .ConfigureAwait(false);
        }

        public async Task RemovePermissionAsync(Role entity, Permission permission, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"RolesPermissions\" WHERE \"role_id\" = @role_id AND  \"permission_id\" = @permission_id",
                    new { })
                .ConfigureAwait(false);
        }
    }
}