using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Role;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Role
{
    public class RoleGetAllOperation : IOperation<RoleGetAll>
    {
        private readonly ILogger<RoleGetAllOperation> _logger;
        private readonly IReadOnlyRoleRepository _repository;

        public RoleGetAllOperation(IReadOnlyRoleRepository permissionRepository,
            ILogger<RoleGetAllOperation> logger)
        {
            _repository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<Result> ExecuteAsync(RoleGetAll request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to get all role");
            try
            {
                return Task.FromResult<Result>(Result.Ok(_repository.GetAllAsync(cancellationToken)));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to execute get all role");
                return Task.FromResult<Result>( Result.Fail(e));
            }
        }
    }
}