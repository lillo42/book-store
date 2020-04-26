using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.User;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.User;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.User
{
    public class UserCreateOperation : IOperation<UserCreate>
    {
        private readonly IUserAggregationStore _aggregationStore;
        private readonly ILogger<UserCreateOperation> _logger;

        public UserCreateOperation(IUserAggregationStore aggregationStore, 
            ILogger<UserCreateOperation> logger)
        {
            _aggregationStore = aggregationStore ?? throw new ArgumentNullException(nameof(aggregationStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(UserCreate request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to create new user. [User: {mail}]", request.Mail);
            try
            {
                var root = _aggregationStore.Create();
                
                var result = root.Create(request.Mail, request.Password, request.IsEnable);
                
                if (result is ErrorResult error)
                {
                    _logger.LogInformation("Invalid information. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }
                
                await _aggregationStore.SaveAsync(root, cancellationToken)
                    .ConfigureAwait(false);
               
                _logger.LogInformation("User create with success. [User: {mail}]", request.Mail);
                
                return Result.Ok((Domain.Common.User)root.State);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to create user. [User: {mail}]", request.Mail);
                return Result.Fail(e);
            }
        }
    }
}