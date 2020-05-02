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
    public class ClientUpdateOperation : IOperation<ClientUpdate>
    {
        private readonly IClientAggregationStore _aggregationStore;
        private readonly ILogger<ClientUpdateOperation> _logger;

        public ClientUpdateOperation(IClientAggregationStore aggregationStore, 
            ILogger<ClientUpdateOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ClientUpdate request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to update client. [Client: {clientId}]", request.Id);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("Client not found. [Client: {clientId}]", request.Id);
                    return DomainError.UserError.NotFound;
                }
                
                var result = root.Update(request.Name, request.ClientId, request.ClientSecret, request.IsEnable);

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Client create with success. [Client: {clientId}]", request.Id);
                
                return Result.Ok((Domain.Common.Client)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to update client. [Client: {clientId}]", request.Id);
                return Result.Fail(e);
            }
        }
    }
}