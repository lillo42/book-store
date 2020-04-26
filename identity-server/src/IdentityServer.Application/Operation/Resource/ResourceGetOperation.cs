using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Resource;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Resource
{
    public class ResourceGetOperation : IOperation<ResourceGetById>
    {
        private readonly IReadOnlyResourceRepository _repository;
        private readonly ILogger<ResourceGetOperation> _logger;

        public ResourceGetOperation(IReadOnlyResourceRepository permissionRepository, 
            ILogger<ResourceGetOperation> logger)
        {
            _repository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ResourceGetById request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to get resource. [Resource: {resourceId}]", request.Id);

            try
            {
                var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);
                
                return Result.Ok(entity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to get resource. [Resource: {resourceId}]", request.Id);
                return Result.Fail(e);
            }
        }
    }
}