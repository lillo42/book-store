using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Permission;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Permission;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Permission
{
    public class PermissionUpdateOperation : IOperation<PermissionUpdate>
    {
        private readonly IPermissionAggregationStore _aggregationStore;
        private readonly ILogger<PermissionUpdateOperation> _logger;

        public PermissionUpdateOperation(IPermissionAggregationStore aggregationStore, 
            ILogger<PermissionUpdateOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(PermissionUpdate request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to update permission. [Permission: {permissionId}]", request.Id);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("Permission not found. [Permission: {permissionId}]", request.Id);
                    return DomainError.PermissionError.NotFound;
                }
                
                var result = root.Update(request.Name, request.DisplayName, request.Description);

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }
                    
                
                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Permission update with success. [Permission: {permissionId}]", request.Id);
                
                return Result.Ok((Domain.Common.Permission)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to update permission.");
                return Result.Fail(e);
            }
        }
    }
}