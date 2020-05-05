using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Permission;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Permission
{
    public class PermissionGetAllOperation : IOperation<PermissionGetAll>
    {
        private readonly ILogger<PermissionGetAllOperation> _logger;
        private readonly IReadOnlyPermissionRepository _permissionRepository;

        public PermissionGetAllOperation(IReadOnlyPermissionRepository permissionRepository,
            ILogger<PermissionGetAllOperation> logger)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(PermissionGetAll request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to get all permission");
            try
            {
                return Result.Ok(_permissionRepository.GetAllAsync(cancellationToken));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to execute get all permission");
                return Result.Fail(e);
            }
        }
    }
}