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
    public class UserRemovePermissionOperation : IOperation<UserRemovePermission>
    {
        private readonly IUserAggregationStore _aggregationStore;
        private readonly ILogger<UserRemovePermissionOperation> _logger;

        public UserRemovePermissionOperation(IUserAggregationStore aggregationStore, 
            ILogger<UserRemovePermissionOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(UserRemovePermission request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to remove permission in user. [User: {userId}][Permission: {permissionId}]", 
                request.Id, request.PermissionId);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("User not found. [User: {userId}]", request.Id);
                    return DomainError.PermissionError.NotFound;
                }
                
                var result = root.RemovePermission(new Domain.Common.Permission(request.PermissionId));

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("User added with success. [User: {userId}][Permission: {permissionId}]", 
                    request.Id, request.PermissionId);
                
                return Result.Ok((Domain.Common.User)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to add permission in user. [User: {userId}][Permission: {permissionId}]", 
                    request.Id, request.PermissionId);
                return Result.Fail(e);
            }
        }
    }
}