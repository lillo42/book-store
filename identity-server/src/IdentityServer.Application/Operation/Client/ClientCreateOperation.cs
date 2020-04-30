using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Client;
using IdentityServer.Application.Request.User;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Client;
using IdentityServer.Domain.Abstractions.User;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Client
{
    public class ClientCreateOperation : IOperation<ClientCreate>
    {
        private readonly IClientAggregationStore _aggregationStore;
        private readonly ILogger<ClientCreateOperation> _logger;

        public ClientCreateOperation(IClientAggregationStore aggregationStore, 
            ILogger<ClientCreateOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ClientCreate request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to create new client. [Client: {clientNam}]", request.Name);
            try
            {
                var root = _aggregationStore.Create();
                
                var result = root.Create(request.Name, request.ClientId, request.ClientSecret, request.IsEnable);
                
                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}][Client: {clientNam}]",
                        error.ErrorCode, request.Name);
                    return error;
                }
                
                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Client create with success. [Client: {clientNam}]", request.Name);
                
                return Result.Ok((Domain.Common.Client)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to create user. [Client: {clientNam}]", request.Name
                );
                return Result.Fail(e);
            }
        }
    }
}