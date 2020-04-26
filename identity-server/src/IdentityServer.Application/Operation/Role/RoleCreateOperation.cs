using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Role;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Role;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Role
{
    public class RoleCreateOperation : IOperation<RoleCreate>
    {
        private readonly IRoleAggregationStore _aggregationStore;
        private readonly ILogger<RoleCreateOperation> _logger;

        public RoleCreateOperation(IRoleAggregationStore aggregationStore, 
            ILogger<RoleCreateOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(RoleCreate request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to create new role. [Permission: {permissionName}]", request.Name);
            try
            {
                var root = _aggregationStore.Create();
                
                var result = root.Create(request.Name, request.DisplayName, request.Description);
                
                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }
                
                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Role create with success. [Permission: {permissionName}]", request.Name);
                
                return Result.Ok((Domain.Common.Role)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to create role.");
                return Result.Fail(e);
            }
        }
    }
}