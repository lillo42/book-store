using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.User;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.User;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.User
{
    public class UserAddPermissionOperation : IOperation<UserAddPermission>
    {
        private readonly IUserAggregationStore _aggregationStore;
        private readonly ILogger<UserAddPermissionOperation> _logger;

        public UserAddPermissionOperation(IUserAggregationStore aggregationStore, 
            ILogger<UserAddPermissionOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(UserAddPermission request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to add permission in user. [User: {userId}][Permission: {permissionId}]", 
                request.Id, request.PermissionId);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("User not found. [Permission: {permissionName}]", request.Id);
                    return DomainError.UserError.NotFound;
                }
                
                var result = await root.AddPermissionAsync(new Domain.Common.Permission(request.PermissionId))
                    .ConfigureAwait(false);

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Permission added with success. [User: {userId}][Permission: {permissionId}]", 
                    request.Id, request.PermissionId);
                
                return Result.Ok((Domain.Common.User)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to add permission in user. [User: {userId}][Permission: {permissionId}]", request.Id, request.PermissionId);
                return Result.Fail(e);
            }
        }
    }
}