using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Resource;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Resource;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Resource
{
    public class ResourceUpdateOperation : IOperation<ResourceUpdate>
    {
        private readonly IResourceAggregationStore _aggregationStore;
        private readonly ILogger<ResourceUpdateOperation> _logger;

        public ResourceUpdateOperation(IResourceAggregationStore aggregationStore, 
            ILogger<ResourceUpdateOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ResourceUpdate request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to update resource. [Resource: {resourceId}]", request.Id);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("Resource not found. [Resource: {resourceId}]", request.Id);
                    return DomainError.ResourceError.NotFound;
                }
                
                var result = await root
                    .UpdateAsync(request.Name, request.DisplayName, request.Description, request.IsEnable, cancellationToken)
                    .ConfigureAwait(false);

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }
                    
                
                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Resource update with success. [Resource: {resourceId}]", request.Id);
                
                return Result.Ok((Domain.Common.Resource)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to update resource. [Resource: {resourceId}]", request.Id);
                return Result.Fail(e);
            }
        }
    }
}