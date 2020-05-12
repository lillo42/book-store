using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;

namespace IdentityServer.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task CreateAsync(Permission entity, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            entity.Id = Guid.NewGuid();
            await connection.ExecuteAsync(
                    "INSERT INTO public.\"Permissions\" (\"id\", \"name\", \"display_name\", \"description\") VALUES (:id, :name, :display_name, :description)",
                    new
                    {
                        id = entity.Id, name = entity.Name, display_name = entity.DisplayName,
                        description = entity.Description
                    })
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(Permission entity, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "UPDATE public.\"Permissions\" SET  \"name\" = :name, \"display_name\" = :display_name, \"description\" = :description WHERE \"id\" = :id",
                    new
                    {
                        id = entity.Id, name = entity.Name, display_name = entity.DisplayName,
                        description = entity.Description
                    })
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(Permission entity, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"RolesPermissions\" WHERE \"permission_id\" = :permission_id",
                    new {permission_id = entity.Id})
                .ConfigureAwait(false);

            await connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsPermissions\" WHERE \"permission_id\" = :permission_id",
                    new {permission_id = entity.Id})
                .ConfigureAwait(false);

            await connection.ExecuteAsync(
                    "DELETE FROM public.\"UsersPermissions\" WHERE \"permission_id\" = :permission_id",
                    new {permission_id = entity.Id})
                .ConfigureAwait(false);

            await connection.ExecuteAsync(
                    "DELETE FROM public.\"Permissions\" WHERE \"id\" = @id",
                    new {id = entity.Id})
                .ConfigureAwait(false);
        }

        public async Task<Permission> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.QueryFirstOrDefaultAsync<Permission>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"description\" AS Description FROM public.\"Permissions\" WHERE \"id\" = :id",
                    new {id})
                .ConfigureAwait(false);
        }

        public async IAsyncEnumerable<Permission> GetAllAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            var reader = await connection
                .ExecuteReaderAsync(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"description\" AS Description FROM public.\"Permissions\"")
                .ConfigureAwait(false);
            var parser = reader.GetRowParser<Permission>();

            while (await reader.ReadAsync(cancellationToken)
                .ConfigureAwait(false))
            {
                yield return parser(reader);
            }
        }

        public async Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.ExecuteScalarAsync<bool>(
                    "SELECT TRUE FROM public.\"Permissions\" where \"id\" = :id",
                    new {id})
                .ConfigureAwait(false);
        }

        public async Task<bool> ExistAsync(string permissionName, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.ExecuteScalarAsync<bool>(
                    "SELECT TRUE FROM public.\"Permissions\" where \"name\" = :permissionName",
                    new {permissionName})
                .ConfigureAwait(false);
        }
    }
}