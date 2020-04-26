using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Role;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.User
{
    public class UserGetAllOperation : IOperation<RoleGetAll>
    {
        private readonly ILogger<UserGetAllOperation> _logger;
        private readonly IReadOnlyUserRepository _repository;

        public UserGetAllOperation(IReadOnlyUserRepository permissionRepository,
            ILogger<UserGetAllOperation> logger)
        {
            _repository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(RoleGetAll request, CancellationToken cancellationToken = default)
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