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
    public class RoleUpdateOperation : IOperation<RoleUpdate>
    {
        private readonly IRoleAggregationStore _aggregationStore;
        private readonly ILogger<RoleUpdateOperation> _logger;

        public RoleUpdateOperation(IRoleAggregationStore aggregationStore, 
            ILogger<RoleUpdateOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(RoleUpdate request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to update role. [Role: {roleId}]", request.Id);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("Role not found. [Role: {roleId}]", request.Id);
                    return DomainError.PermissionError.NotFound;
                }
                
                var result = await root.UpdateAsync(request.Name, request.DisplayName, request.Description, cancellationToken)
                    .ConfigureAwait(false);

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}][Role: {roleId}]", error.ErrorCode, request.Id);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Role create with success. [Role: {roleId}]", request.Id);
                
                return Result.Ok((Domain.Common.Role)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to update role. [Role: {roleId}]", request.Id);
                return Result.Fail(e);
            }
        }
    }
}