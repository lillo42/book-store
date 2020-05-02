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
    public class UserRemoveRoleOperation : IOperation<UserRemoveRole>
    {
        private readonly IUserAggregationStore _aggregationStore;
        private readonly ILogger<UserRemoveRoleOperation> _logger;

        public UserRemoveRoleOperation(IUserAggregationStore aggregationStore, 
            ILogger<UserRemoveRoleOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(UserRemoveRole request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to remove role in user. [User: {userId}][Role: {roleId}]", 
                request.Id, request.RoleId);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("User not found. [User: {userId}]", request.Id);
                    return DomainError.UserError.NotFound;
                }
                
                var result = root.RemoveRole(new Domain.Common.Role(request.RoleId));

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("Role remove with success. [User: {userId}][Role: {roleId}]", 
                    request.Id, request.RoleId);
                
                return Result.Ok((Domain.Common.User)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to remove role in user. [User: {userId}][Role: {roleId}]", request.Id, request.RoleId);
                return Result.Fail(e);
            }
        }
    }
}