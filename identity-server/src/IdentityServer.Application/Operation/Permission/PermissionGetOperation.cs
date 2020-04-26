using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Permission;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Permission
{
    public class PermissionGetOperation : IOperation<PermissionGetById>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly ILogger<PermissionGetOperation> _logger;

        public PermissionGetOperation(IPermissionRepository permissionRepository, 
            ILogger<PermissionGetOperation> logger)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(PermissionGetById request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to get permission. [Permission: {permissionId}]", request.Id);

            try
            {
                var permissions = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);
                
                return Result.Ok(permissions);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to get permission");
                return Result.Fail(e);
            }
        }
    }
}