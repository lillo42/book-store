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
    public class ClientRemoveRoleOperation : IOperation<ClientRemoveRole>
    {
        private readonly IClientAggregationStore _aggregationStore;
        private readonly ILogger<ClientRemoveRoleOperation> _logger;

        public ClientRemoveRoleOperation(IClientAggregationStore aggregationStore, 
            ILogger<ClientRemoveRoleOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ClientRemoveRole request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to remove role in client. [Client: {clientId}][Role: {roleId}]", 
                request.Id, request.RoleId);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("User not found. [User: {userId}]", request.Id);
                    return DomainError.ClientError.NotFound;
                }
                
                var result = root.RemoveRole(new Domain.Common.Role(request.RoleId));

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Role remove with success. [Client: {clientId}][Role: {roleId}]", 
                    request.Id, request.RoleId);
                
                return Result.Ok((Domain.Common.Client)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to remove role in client. [Client: {clientId}][Role: {roleId}]",
                    request.Id, request.RoleId);
                return Result.Fail(e);
            }
        }
    }
}