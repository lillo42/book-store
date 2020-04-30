using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Client;
using IdentityServer.Application.Request.User;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Client;
using IdentityServer.Domain.Abstractions.User;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Client
{
    public class ClientAddRoleOperation : IOperation<ClientAddRole>
    {
        private readonly IClientAggregationStore _aggregationStore;
        private readonly ILogger<ClientAddRoleOperation> _logger;

        public ClientAddRoleOperation(IClientAggregationStore aggregationStore, 
            ILogger<ClientAddRoleOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ClientAddRole request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to add role in client. [Client: {clientId}][Role: {roleId}]", 
                request.Id, request.RoleId);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("Client not found. [Client: {clientId}]", request.Id);
                    return DomainError.PermissionError.NotFound;
                }
                
                var result = await root.AddRoleAsync(new Domain.Common.Role(request.RoleId))
                    .ConfigureAwait(false);

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Role added with success. [Client: {clientId}][Role: {roleId}]", 
                    request.Id, request.RoleId);
                
                return Result.Ok((Domain.Common.Client)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to add role in client. [Client: {clientId}][Role: {roleId}]",
                    request.Id, request.RoleId);
                return Result.Fail(e);
            }
        }
    }
}