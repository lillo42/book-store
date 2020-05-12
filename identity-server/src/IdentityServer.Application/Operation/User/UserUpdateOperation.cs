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
    public class UserUpdateOperation : IOperation<UserUpdate>
    {
        private readonly IUserAggregationStore _aggregationStore;
        private readonly ILogger<UserUpdateOperation> _logger;

        public UserUpdateOperation(IUserAggregationStore aggregationStore, 
            ILogger<UserUpdateOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(UserUpdate request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to update user. [User: {userId}]", request.Id);
            try
            {
                var root = await _aggregationStore.GetAsync(request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (root == null)
                {
                    _logger.LogInformation("User not found. [User: {userId}]", request.Id);
                    return DomainError.UserError.NotFound;
                }
                
                var result = await root.UpdateAsync(request.Mail, request.IsEnable, cancellationToken)
                    .ConfigureAwait(false);

                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("User create with success. [User: {userId}]", request.Id);
                
                return Result.Ok((Domain.Common.User)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to update user. [User: {userId}]", request.Id);
                return Result.Fail(e);
            }
        }
    }
}