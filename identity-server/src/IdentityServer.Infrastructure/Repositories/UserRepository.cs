using System;
using System.Collections.Generic;
using System.Data.Common;
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
    public class UserRepository : IUserRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task CreateAsync(User entity, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            entity.Id = Guid.NewGuid();
            await connection.ExecuteAsync(
                    "INSERT INTO public.\"Users\" (\"id\", \"mail\", \"password\", \"is_active\") VALUES (:id, :mail, :password, :is_active)",
                    new { id = entity.Id, mail = entity.Mail, password = entity.Password, is_active = entity.IsEnable})
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "UPDATE public.\"Users\" SET  \"mail\" = @mail, \"is_active\" = :is_active WHERE \"id\" = :id",
                    new { id = entity.Id, mail = entity.Mail, is_active = entity.IsEnable})
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"Users\" WHERE \"id\" = :id",
                    new { id = entity.Id})
                .ConfigureAwait(false);
        }

        public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            var multi = await connection.QueryMultipleAsync(
                    $@"SELECT 
                            ""id"" AS Id,
                            ""mail"" AS Mail,
                            ""is_active"" AS active,
                        FROM public.""Users"" 
                        WHERE ""id"" = :id
                        LIMIT 1;
                        SELECT
                            R.""id"" AS Id,
                            R.""name"" AS Name,
                        FROM public.""Roles"" R
                        INNER JOIN public.""UsersRoles"" UR ON R.""id"" = UR.""role_id""
                        WHERE UR.""user_id"" = :id;
                        SELECT
                            P.""id"" AS Id,
                            P.""name"" AS Name,
                        FROM public.""Permissions"" P
                        INNER JOIN public.""UsersPermissions"" UP ON P.""id"" = UP.""permission_id""
                        WHERE UR.""user_id"" = :id;",
                    new {id})
                .ConfigureAwait(false);

            var user = await  multi.ReadFirstOrDefaultAsync<User>()
                .ConfigureAwait(false);

            if (user == null)
            {
                return null;
            }

            var roles = await multi.ReadAsync<Role>()
                .ConfigureAwait(false);

            var permission = await multi.ReadAsync<Permission>()
                .ConfigureAwait(false);

            user.Roles = roles.ToHashSet();
            user.Permissions = permission.ToHashSet();

            return user;
        }

        public async IAsyncEnumerable<User> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            var reader = await connection.ExecuteReaderAsync($@"SELECT 
                            ""id"" AS Id,
                            ""mail"" AS Mail,
                            ""is_active"" AS active,
                        FROM public.""Users""")
                .ConfigureAwait(false);

            var parse = reader.GetRowParser<User>();

            while (await reader.ReadAsync(cancellationToken)
                .ConfigureAwait(false))
            {
                var user = parse(reader);
                var multi = await connection.QueryMultipleAsync($@"
                        SELECT
                            R.""id"" AS Id,
                            R.""name"" AS Name,
                            R.""display_name"" AS DisplayName,
                             R.""description"" AS Description
                        FROM public.""Roles"" R
                        INNER JOIN public.""UsersRoles"" UR ON R.""id"" = UR.""role_id""
                        WHERE UR.""user_id"" = :id;
                        SELECT
                            P.""id"" AS Id,
                            P.""name"" AS Name,
                            P.""display_name"" AS DisplayName,
                            P.""description"" AS Description
                        FROM public.""Permissions"" P
                        INNER JOIN public.""UsersPermissions"" UP ON P.""id"" = UP.""permission_id""
                        WHERE UR.""user_id"" = :id;
", new {id = user.Id}).ConfigureAwait(false);

                var roles = await multi.ReadAsync<Role>()
                    .ConfigureAwait(false);

                var permission = await multi.ReadAsync<Permission>()
                    .ConfigureAwait(false);

                user.Roles = roles?.ToHashSet();
                user.Permissions = permission?.ToHashSet();

                yield return user;
            }
        }

        public async Task<User> GetByMailAndPasswordAsync(string mail, string password, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            var user = await connection.QueryFirstOrDefaultAsync<User>($@"
                        SELECT 
                            ""id"" AS Id,
                            ""mail"" AS Mail,
                            ""is_active"" AS active,
                        FROM public.""Users"" 
                        WHERE ""mail"" = :mail AND password = :password 
                        LIMIT 1;", new { mail, password})
                .ConfigureAwait(false);

            if (user == null)
            {
                return null;
            }
            
            var multi = await connection.QueryMultipleAsync(
                    $@"SELECT
                            R.""id"" AS Id,
                            R.""name"" AS Name,
                        FROM public.""Roles"" R
                        INNER JOIN public.""UsersRoles"" UR ON R.""id"" = UR.""role_id""
                        WHERE UR.""user_id"" = :id;
                        SELECT
                            P.""id"" AS Id,
                            P.""name"" AS Name,
                        FROM public.""Permissions"" P
                        INNER JOIN public.""UsersPermissions"" UP ON P.""id"" = UP.""permission_id""
                        WHERE UR.""user_id"" = :id;
",
                    new {id = user.Id})
                .ConfigureAwait(false);
            
            var roles = await multi.ReadAsync<Role>()
                .ConfigureAwait(false);

            var permission = await multi.ReadAsync<Permission>()
                .ConfigureAwait(false);

            user.Roles = roles?.ToHashSet();
            user.Permissions = permission?.ToHashSet();

            return user;
        }

        public async Task<bool> IsEnableAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.ExecuteScalarAsync<bool>("SELECT \"is_active\" FROM  public.\"Users\" WHERE id = :id LIMIT 1",
                    new {id})
                .ConfigureAwait(false);
        }

        public async Task AddPermissionAsync(User entity, Permission permission, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "INSERT INTO public.\"UsersPermissions\" (\"user_id\", \"permission_id\") VALUES (:user_id, :permission_id);",
                    new { user_id = entity.Id, permission_id = permission.Id })
                .ConfigureAwait(false);
        }

        public async Task RemovePermissionAsync(User entity, Permission permission, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"UsersPermissions\" WHERE \"user_id\" = :user_id AND  \"permission_id\" = :permission_id",
                    new { user_id = entity.Id, permission_id = permission.Id})
                .ConfigureAwait(false);
        }

        public async Task AddRoleAsync(User entity, Role role, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "INSERT INTO public.\"UsersRoles\" (\"user_id\", \"role_id\") VALUES (:user_id, :role_id);",
                    new { user_id = entity.Id, role_id = role.Id })
                .ConfigureAwait(false);   
        }

        public async Task RemoveRoleAsync(User entity, Role role, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"UsersRoles\" WHERE \"user_id\" = :user_id AND  \"role_id\" = :role_id",
                    new { user_id = entity.Id, role_id = role.Id})
                .ConfigureAwait(false);
        }
    }
}