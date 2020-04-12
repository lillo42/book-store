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
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection _connection;

        public UserRepository(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task CreateAsync(User entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Guid.NewGuid();
            await _connection.ExecuteAsync(
                    "INSERT INTO public.\"Users\" (\"id\", \"mail\", \"password\", \"is_active\") VALUES (@id, @mail, @password, @is_active)",
                    new { id = entity.Id, mail = entity.Mail, password = entity.Password, is_active = entity.IsEnable})
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "UPDATE public.\"Users\" SET  \"mail\" = @mail, \"is_active\" = @is_active WHERE \"id\" = @id",
                    new { id = entity.Id, mail = entity.Mail, is_active = entity.IsEnable})
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"Users\" WHERE \"id\" = @id",
                    new { id = entity.Id})
                .ConfigureAwait(false);
        }

        public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var multi = await _connection.QueryMultipleAsync(
                    $@"SELECT 
                            ""id"" AS Id,
                            ""mail"" AS Mail,
                            ""is_active"" AS active,
                        FROM public.""Users"" 
                        WHERE ""id"" = @id
                        LIMIT 1;
                        SELECT
                            R.""id"" AS Id,
                            R.""name"" AS Name,
                        FROM public.""Roles"" R
                        INNER JOIN public.""UsersRoles"" UR ON R.""id"" = UR.""role_id""
                        WHERE UR.""user_id"" = @id;
                        SELECT
                            P.""id"" AS Id,
                            P.""name"" AS Name,
                        FROM public.""Permissions"" P
                        INNER JOIN public.""UsersPermissions"" UP ON P.""id"" = UP.""permission_id""
                        WHERE UR.""user_id"" = @id;
",
                    new {id})
                .ConfigureAwait(false);

            var user = await  multi.ReadFirstOrDefaultAsync<User>()
                .ConfigureAwait(false);

            var roles = await multi.ReadAsync<Role>()
                .ConfigureAwait(false);

            var permission = await multi.ReadAsync<Permission>()
                .ConfigureAwait(false);

            user.Roles = roles.ToHashSet();
            user.Permissions = permission.ToHashSet();

            return user;
        }

        public async Task<User> GetByMailAndPasswordAsync(string mail, string password, CancellationToken cancellationToken = default)
        {
            var user = await _connection.QueryFirstOrDefaultAsync<User>($@"
                        SELECT 
                            ""id"" AS Id,
                            ""mail"" AS Mail,
                            ""is_active"" AS active,
                        FROM public.""Users"" 
                        WHERE ""mail"" = @mail AND password = @password 
                        LIMIT 1;", new { mail, password})
                .ConfigureAwait(false);

            if (user == null)
            {
                return null;
            }
            
            var multi = await _connection.QueryMultipleAsync(
                    $@"SELECT
                            R.""id"" AS Id,
                            R.""name"" AS Name,
                        FROM public.""Roles"" R
                        INNER JOIN public.""UsersRoles"" UR ON R.""id"" = UR.""role_id""
                        WHERE UR.""user_id"" = @id;
                        SELECT
                            P.""id"" AS Id,
                            P.""name"" AS Name,
                        FROM public.""Permissions"" P
                        INNER JOIN public.""UsersPermissions"" UP ON P.""id"" = UP.""permission_id""
                        WHERE UR.""user_id"" = @id;
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
            return await _connection.ExecuteScalarAsync<bool>("SELECT \"is_active\" FROM  public.\"Users\" WHERE id = @id LIMIT 1",
                    new {id})
                .ConfigureAwait(false);
        }

        public async Task AddPermissionsAsync(User entity, CancellationToken cancellationToken = default)
        {
            foreach (var permission in entity.Permissions ?? Enumerable.Empty<Permission>())
            {
                await _connection.ExecuteAsync(
                        "INSERT INTO public.\"UsersPermissions\" (\"user_id\", \"permission_id\") VALUES (@user_id, @permission_id) ON CONFLICT DO NOTHING;",
                        new { user_id = entity.Id, permission_id = permission.Id })
                    .ConfigureAwait(false);   
            }
        }

        public async Task RemovePermissionsAsync(User entity, CancellationToken cancellationToken = default)
        {
            if (entity.Permissions.Count == 0)
            {
                return;
            }

            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"UsersPermissions\" WHERE \"user_id\" = @user_id AND  \"permission_id\" NOT IN @permisions",
                    new { user_id = entity.Id, permisions = entity.Permissions.Select(x=> x.Id).ToArray()})
                .ConfigureAwait(false);
        }

        public async Task AddRolesAsync(User entity, CancellationToken cancellationToken = default)
        {
            foreach (var permission in entity.Roles ?? Enumerable.Empty<Role>())
            {
                await _connection.ExecuteAsync(
                        "INSERT INTO public.\"UsersRoles\" (\"user_id\", \"role_id\") VALUES (@user_id, @role_id) ON CONFLICT DO NOTHING;",
                        new { user_id = entity.Id, role_id = permission.Id })
                    .ConfigureAwait(false);   
            }
        }

        public async Task RemoveRolesAsync(User entity, CancellationToken cancellationToken = default)
        {
            if (entity.Roles.Count == 0)
            {
                return;
            }

            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"UsersRoles\" WHERE \"user_id\" = @user_id AND  \"role_id\" NOT IN @roles",
                    new { user_id = entity.Id, roles = entity.Permissions.Select(x=> x.Id).ToArray()})
                .ConfigureAwait(false);
        }
    }
}