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
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"is_active\" AS IsEnable  FROM public.\"Resources\" WHERE \"id\" = @id",
                    new {id})
                .ConfigureAwait(false);

        public async Task<Resource> GetByNameAsync(string name, CancellationToken cancellationToken = default) 
            => await _connection.QueryFirstOrDefaultAsync<Resource>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"is_active\" AS IsEnable  FROM public.\"Resources\" WHERE \"name\" = @name",
                    new {name})
                .ConfigureAwait(false);

        public async Task<IEnumerable<Resource>> GetAllAsync(CancellationToken cancellationToken = default) 
            => await _connection.QueryAsync<Resource>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"description\" AS Description,  \"is_active\" AS IsEnable  FROM public.\"Resources\"")
                .ConfigureAwait(false);

        public async Task<IEnumerable<Resource>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default) 
            => await _connection.QueryAsync<Resource>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"is_active\" AS IsEnable  FROM public.\"Resources\" WHERE \"name\" IN @names",
                    new {names})
                .ConfigureAwait(false);

        public async Task<bool> ExistAsync(Guid resourceId, CancellationToken cancellationToken = default) 
            => await _connection.ExecuteScalarAsync<bool>(
                    "SELECT TRUE FROM public.\"Permissions\" where \"id\" = :id LIMIT 1;",
                    new { id =  resourceId })
                .ConfigureAwait(false);

        public async Task CreateAsync(Resource entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Guid.NewGuid();
            await _connection.ExecuteAsync(
                    "INSERT INTO public.\"Resources\" (\"id\", \"name\", \"display_name\", \"description\",  \"is_active\") VALUES (:id, :name, :display_name, :description, :is_active)",
                    new
                    {
                        id = entity.Id, 
                        name = entity.Name, 
                        display_name = entity.DisplayName, 
                        description = entity.Description,
                        is_active = entity.IsEnable
                    })
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(Resource entity, CancellationToken cancellationToken = default) 
            => await _connection.ExecuteAsync(
                    "UPDATE public.\"Resources\" SET \"name\" = @name, \"display_name\" = @display_name,  \"is_active\" = @is_active WHERE \"id\" = @id",
                    new { id = entity.Id, name = entity.Name, display_name = entity.DisplayName, is_active = entity.IsEnable})
                .ConfigureAwait(false);

        public async Task DeleteAsync(Resource entity, CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsResources\" WHERE \"resource_id\" = @resource_id",
                    new { resource_id = entity.Id})
                .ConfigureAwait(false);
            
            await _connection.ExecuteAsync(
                    "DELETE FROM public.\"Resources\" WHERE \"id\" = @id",
                    new { id = entity.Id})
                .ConfigureAwait(false);
        } 
    }
}