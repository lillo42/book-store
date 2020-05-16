using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Client;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Client;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Client
{
    public class ClientRemoveResourceOperation : IOperation<ClientRemoveResource>
    {
        private readonly IClientAggregationStore _aggregationStore;
        private readonly ILogger<ClientRemoveResourceOperation> _logger;

        public ClientRemoveResourceOperation(IClientAggregationStore aggregationStore, 
            ILogger<ClientRemoveResourceOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ClientRemoveResource request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to remove resource in client. [Client: {clientId}][Resource: {resourceId}]", 
                request.Id, request.ResourceId);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("Client not found. [Client: {clientId}]", request.Id);
                    return DomainError.ClientError.NotFound;
                }
                
                var result = root.RemoveResource(new Domain.Common.Resource(request.ResourceId));

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Resource removed with success. [Client: {clientId}][Resource: {resourceId}]", 
                    request.Id, request.ResourceId);
                
                return Result.Ok((Domain.Common.Client)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to add resource in client. [Client: {clientId}][Resource: {resourceId}]", 
                    request.Id, request.ResourceId);
                return Result.Fail(e);
            }
        }
    }
}