using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Client;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Client
{
    public class ClientGetOperation : IOperation<ClientGetById>
    {
        private readonly IReadOnlyClientRepository _repository;
        private readonly ILogger<ClientGetOperation> _logger;

        public ClientGetOperation(IReadOnlyClientRepository permissionRepository, ILogger<ClientGetOperation> logger)
        {
            _repository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ClientGetById request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to get client. [Client: {clientId}]", request.Id);
            try
            {
                var role = await _repository.GetByIdAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (role == null)
                {
                    _logger.LogInformation("Client not found. [Client: {clientId}]", request.Id);
                    return DomainError.UserError.NotFound;
                }
                
                return Result.Ok(role);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to get client. [Client: {clientId}]", request.Id);
                return Result.Fail(e);
            }
        }
    }
}