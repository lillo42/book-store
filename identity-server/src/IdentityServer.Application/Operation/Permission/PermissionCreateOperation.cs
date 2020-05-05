using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Permission;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Permission;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Permission
{
    public class PermissionCreateOperation : IOperation<PermissionCreate>
    {
        private readonly IPermissionAggregationStore _aggregationStore;
        private readonly ILogger<PermissionCreateOperation> _logger;

        public PermissionCreateOperation(IPermissionAggregationStore aggregationStore, 
            ILogger<PermissionCreateOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(PermissionCreate request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to create new permission. [Permission: {permissionName}]", request.Name);
            try
            {
                var root = _aggregationStore.Create();
                
                var result = await root.CreateAsync(request.Name, request.DisplayName, request.Description, cancellationToken)
                    .ConfigureAwait(false);
                
                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }
                
                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Permission create with success. [Permission: {permissionName}]", request.Name);
                
                return Result.Ok((Domain.Common.Permission)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to create permission.");
                return Result.Fail(e);
            }
        }
    }
}