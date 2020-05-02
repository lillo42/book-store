using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Npgsql;

namespace IdentityServer.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DbConnection _connection;

        public PermissionRepository(DbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task CreateAsync(Permission entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Guid.NewGuid();
            await _connection.ExecuteAsync(
                    "INSERT INTO public.\"Permissions\" (\"id\", \"name\", \"display_name\", \"description\") VALUES (:id, :name, :display_name, :description)", 
                    new { id = entity.Id, name = entity.Name,  display_name = entity.DisplayName, description = entity.Description })
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(Permission entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "UPDATE public.\"Permissions\" SET  \"name\" = :name, \"display_name\" = :display_name, \"description\" = :description WHERE \"id\" = :id",
                    new { id = entity.Id, name = entity.Name,  display_name = entity.DisplayName, description = entity.Description })
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(Permission entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"RolesPermissions\" WHERE \"permission_id\" = :permission_id", 
                    new { permission_id = entity.Id })
                .ConfigureAwait(false);
            
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsPermissions\" WHERE \"permission_id\" = :permission_id", 
                    new { permission_id = entity.Id })
                .ConfigureAwait(false);
            
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"UsersPermissions\" WHERE \"permission_id\" = :permission_id", 
                    new { permission_id = entity.Id })
                .ConfigureAwait(false);
            
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"Permissions\" WHERE \"id\" = @id", 
                    new { id = entity.Id })
                .ConfigureAwait(false);
        }

        public async Task<Permission> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) 
            => await _connection.QueryFirstOrDefaultAsync<Permission>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"description\" AS Description FROM public.\"Permissions\" WHERE \"id\" = :id", 
                    new { id })
                .ConfigureAwait(false);

        public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default) 
            => await _connection.QueryAsync<Permission>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"description\" AS Description FROM public.\"Permissions\"")
                .ConfigureAwait(false);

        public async Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default) 
            => await _connection.ExecuteScalarAsync<bool>(
                    "SELECT TRUE FROM public.\"Permissions\" where \"id\" = :id",
                    new {id})
                .ConfigureAwait(false);
    }
}