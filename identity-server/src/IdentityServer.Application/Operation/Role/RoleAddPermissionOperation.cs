using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Role;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Role;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Role
{
    public class RoleAddPermissionOperation : IOperation<RoleAddPermission>
    {
        private readonly IRoleAggregationStore _aggregationStore;
        private readonly ILogger<RoleAddPermissionOperation> _logger;

        public RoleAddPermissionOperation(IRoleAggregationStore aggregationStore, 
            ILogger<RoleAddPermissionOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(RoleAddPermission request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to add permission in role. [Role: {roleId}][Permission: {permissionId}]", 
                request.Id, request.PermissionId);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("Role not found. [Permission: {permissionName}]", request.Id);
                    return DomainError.PermissionError.NotFound;
                }
                
                var result = await root.AddPermission(new Domain.Common.Permission(request.PermissionId))
                    .ConfigureAwait(false);

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Permission added with success. [Role: {roleId}][Permission: {permissionId}]", 
                    request.Id, request.PermissionId);
                
                return Result.Ok((Domain.Common.Role)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to add permission in role.");
                return Result.Fail(e);
            }
        }
    }
}