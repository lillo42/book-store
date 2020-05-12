using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;

namespace IdentityServer.Infrastructure.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResourceRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }


        public async Task<Resource> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.QueryFirstOrDefaultAsync<Resource>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"description\" AS Description, \"is_active\" AS IsEnable  FROM public.\"Resources\" WHERE \"id\" = @id",
                    new {id})
                .ConfigureAwait(false);
        }

        public async Task<Resource> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.QueryFirstOrDefaultAsync<Resource>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"description\" AS Description, \"is_active\" AS IsEnable  FROM public.\"Resources\" WHERE \"name\" = @name",
                    new {name})
                .ConfigureAwait(false);
        }

        public async IAsyncEnumerable<Resource> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            var reader = await connection.ExecuteReaderAsync(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"description\" AS Description,  \"is_active\" AS IsEnable  FROM public.\"Resources\"")
                .ConfigureAwait(false);

            var parse = reader.GetRowParser<Resource>();

            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return parse(reader);
            }
        }

        public async Task<IEnumerable<Resource>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.QueryAsync<Resource>(
                    "SELECT \"id\" AS Id, \"name\" AS Name, \"display_name\" AS DisplayName, \"description\" AS Description, \"is_active\" AS IsEnable  FROM public.\"Resources\" WHERE \"name\" IN @names",
                    new {names})
                .ConfigureAwait(false);
        }

        public async Task<bool> ExistAsync(Guid resourceId, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.ExecuteScalarAsync<bool>(
                    "SELECT TRUE FROM public.\"Resources\" where \"id\" = :id LIMIT 1;",
                    new {id = resourceId})
                .ConfigureAwait(false);
        }

        public async Task<bool> ExistAsync(string resourceName, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            return await connection.ExecuteScalarAsync<bool>(
                    "SELECT TRUE FROM public.\"Resources\" where \"name\" = :resourceName LIMIT 1;",
                    new {resourceName})
                .ConfigureAwait(false);
        }

        public async Task CreateAsync(Resource entity, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            entity.Id = Guid.NewGuid();
            await connection.ExecuteAsync(
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
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "UPDATE public.\"Resources\" SET \"name\" = @name, \"display_name\" = @display_name,  \"is_active\" = @is_active WHERE \"id\" = @id",
                    new
                    {
                        id = entity.Id, name = entity.Name, display_name = entity.DisplayName,
                        is_active = entity.IsEnable
                    })
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(Resource entity, CancellationToken cancellationToken = default)
        {
            var connection = await _unitOfWork.GetOrCreateDbConnection(cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"ClientsResources\" WHERE \"resource_id\" = @resource_id",
                    new { resource_id = entity.Id})
                .ConfigureAwait(false);
            
            await connection.ExecuteAsync(
                    "DELETE FROM public.\"Resources\" WHERE \"id\" = @id",
                    new { id = entity.Id})
                .ConfigureAwait(false);
        } 
    }
}