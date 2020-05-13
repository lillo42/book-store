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
    public class ClientAddResourceOperation : IOperation<ClientAddResource>
    {
        private readonly IClientAggregationStore _aggregationStore;
        private readonly ILogger<ClientAddResourceOperation> _logger;

        public ClientAddResourceOperation(IClientAggregationStore aggregationStore, 
            ILogger<ClientAddResourceOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ClientAddResource request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to add resource in client. [Client: {clientId}][Resource: {resourceId}]", 
                request.Id, request.ResourceId);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("Client not found. [Client: {clientId}]", request.Id);
                    return DomainError.UserError.NotFound;
                }
                
                var result = await root.AddResourceAsync(new Domain.Common.Resource(request.ResourceId), cancellationToken)
                    .ConfigureAwait(false);

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Resource added in success. [Client: {clientId}][Resource: {resourceId}]", 
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