using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Resource;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Resource;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Resource
{
    public class ResourceCreateOperation : IOperation<ResourceCreate>
    {
        private readonly IResourceAggregationStore _aggregationStore;
        private readonly ILogger<ResourceCreateOperation> _logger;

        public ResourceCreateOperation(IResourceAggregationStore aggregationStore, 
            ILogger<ResourceCreateOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ResourceCreate request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to create new resource. [Resource: {resourceName}]", request.Name);
            try
            {
                var root = _aggregationStore.Create();
                
                var result = await root
                    .CreateAsync(request.Name, request.DisplayName, request.Description, request.IsEnable, cancellationToken)
                    .ConfigureAwait(false);
                
                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }
                
                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Resource create with success. [Resource: {resourceName}]", request.Name);

                return Result.Ok((Domain.Common.Resource) root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to create permission.");
                return Result.Fail(e);
            }
        }
    }
}