using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Role;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Role
{
    public class RoleGetOperation : IOperation<RoleGetById>
    {
        private readonly IRoleRepository _repository;
        private readonly ILogger<RoleGetOperation> _logger;

        public RoleGetOperation(IRoleRepository permissionRepository, 
            ILogger<RoleGetOperation> logger)
        {
            _repository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(RoleGetById request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to get role. [Role: {roleId}]", request.Id);
            try
            {
                var role = await _repository.GetByIdAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (role == null)
                {
                    _logger.LogInformation("Role not found. [Role: {roleId}]", request.Id);
                    return DomainError.RoleError.NotFound;
                }
                
                return Result.Ok(role);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to get role. [Role: {roleId}]", request.Id);
                return Result.Fail(e);
            }
        }
    }
}