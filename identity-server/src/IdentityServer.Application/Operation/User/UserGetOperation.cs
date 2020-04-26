using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.User;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.User
{
    public class UserGetOperation : IOperation<UserGetById>
    {
        private readonly IRoleRepository _repository;
        private readonly ILogger<UserGetOperation> _logger;

        public UserGetOperation(IRoleRepository permissionRepository, ILogger<UserGetOperation> logger)
        {
            _repository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(UserGetById request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to get user. [User: {userId}]", request.Id);
            try
            {
                var role = await _repository.GetByIdAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (role == null)
                {
                    _logger.LogInformation("User not found. [User: {userId}]", request.Id);
                    return DomainError.UserError.NotFound;
                }
                
                return Result.Ok(role);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to get user. [User: {userId}]", request.Id);
                return Result.Fail(e);
            }
        }
    }
}