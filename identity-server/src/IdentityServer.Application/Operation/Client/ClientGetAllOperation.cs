using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Client;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Client
{
    public class ClientGetAllOperation : IOperation<ClientGetAll>
    {
        private readonly ILogger<ClientGetAllOperation> _logger;
        private readonly IReadOnlyClientRepository _repository;

        public ClientGetAllOperation(IReadOnlyClientRepository permissionRepository,
            ILogger<ClientGetAllOperation> logger)
        {
            _repository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ClientGetAll request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to get all user");
            try
            {
                return Result.Ok(_repository.GetAllAsync(cancellationToken));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to execute get all user");
                return Result.Fail(e);
            }
        }
    }
}