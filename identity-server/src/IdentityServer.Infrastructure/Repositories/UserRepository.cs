using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Infrastructure.Entities;
using Npgsql;

namespace IdentityServer.Infrastructure.Repositories
{
    public class UserRepository : IReadOnlyUsersRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        
        public async Task<Guid> GetByEmailAsync(string mail, string password)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            
            return await connection.QueryFirstOrDefaultAsync<Guid>($@"
            SELECT 
                   U.""id"" AS Id 
            FROM public.""Users"" U
            WHERE U.""mail"" = @mail AND U.""password"" = @password ", new { mail, password })
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Role>> GetUsersRoles(Guid id)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Role>($@"
                SELECT
                    R.""id"" as Id,
                    R.""name"" as Name
                FROM public.""UsersRoles"" UR
                INNER JOIN public.""Roles"" R ON UR.""role_id"" = R.""id""
                WHERE UR.""user_id"" = @userId", new { userId = id})
                    .ConfigureAwait(false);
        }

        public async Task<bool> IsUserEnableAsync(Guid id)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<bool>("SELECT \"is_active\" FROM public.\"Users\"")
                .ConfigureAwait(false);
        }
    }
}