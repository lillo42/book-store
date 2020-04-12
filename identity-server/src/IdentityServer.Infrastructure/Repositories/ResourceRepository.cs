using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Npgsql;

namespace IdentityServer.Infrastructure.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly NpgsqlConnection _connection;

        public ResourceRepository(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<Resource> GetByIdAsync(Guid id, CancellationToken cancellation = default) 
            => await _connection.QueryFirstOrDefaultAsync<Resource>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"enable\" AS IsEnable  FROM public.\"Resource\" WHERE \"id\" = @id",
                    new {id})
                .ConfigureAwait(false);

        public async Task<Resource> GetByNameAsync(string name, CancellationToken cancellationToken = default) 
            => await _connection.QueryFirstOrDefaultAsync<Resource>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"enable\" AS IsEnable  FROM public.\"Resource\" WHERE \"name\" = @name",
                    new {name})
                .ConfigureAwait(false);

        public async Task<IEnumerable<Resource>> GetAllAsync(CancellationToken cancellationToken = default) 
            => await _connection.QueryAsync<Resource>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"enable\" AS IsEnable  FROM public.\"Resource\"")
                .ConfigureAwait(false);

        public async Task<IEnumerable<Resource>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default) 
            => await _connection.QueryAsync<Resource>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"enable\" AS IsEnable  FROM public.\"Resource\" WHERE \"name\" IN @names",
                    new {names})
                .ConfigureAwait(false);

        public async Task CreateAsync(Resource entity, CancellationToken cancellationToken = default) 
            => await _connection.QueryFirstOrDefaultAsync<Resource>(
                    "INSERT INTO public.\"Resource\" (\"id\", \"name\", \"display_name\", \"enable\") VALUES (@id, @name, @display_name, @enable)",
                    new { id = entity.Id, name = entity.Name, display_name = entity.DisplayName, enable = entity.IsEnable})
                .ConfigureAwait(false);

        public async Task UpdateAsync(Resource entity, CancellationToken cancellationToken = default) 
            => await _connection.QueryFirstOrDefaultAsync<Resource>(
                    "UPDATE public.\"Resource\" SET \"name\" = @name, \"display_name\" = @display_name,  \"enable\" = @enable WHERE \"id\" = @id",
                    new { id = entity.Id, name = entity.Name, display_name = entity.DisplayName, enable = entity.IsEnable})
                .ConfigureAwait(false);

        public async Task DeleteAsync(Resource entity, CancellationToken cancellationToken = default) 
            => await _connection.QueryFirstOrDefaultAsync<Resource>(
                    "DELETE FROM public.\"Resource\" WHERE \"id\" = @id",
                    new { id = entity.Id})
                .ConfigureAwait(false);
    }
}