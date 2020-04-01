using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Users.Application.Contracts.Request;
using Users.Application.Contracts.Response;
using Users.Application.Mapper;
using Users.Domain;

namespace Users.Application.Operations
{
    public class UserUpdateOperation : IOperation<UserUpdate>
    {
        private readonly IUserAggregateStore _store;
        private readonly ILogger<UserUpdateOperation> _logger;
        private readonly IMapper<Domain.Common.User, User> _mapper;

        public UserUpdateOperation(IUserAggregateStore store, 
            ILogger<UserUpdateOperation> logger, 
            IMapper<Domain.Common.User, User> mapper)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async ValueTask<Result> ExecuteAsync(UserUpdate operation, CancellationToken cancellation = default)
        {
            var scope = _logger.BeginScope("Updating user. [UserId: {userId}]", operation.Id);
            try
            {
                var root = await _store.GetAsync(operation.Id, cancellation);

                if (root == null)
                {
                    _logger.LogInformation("User not found");
                    return DomainError.UserError.UserNotFound;
                }

                if (root.Update(operation.FirstName, operation.LastNames, operation.BirthDate) is ErrorResult error)
                {
                    _logger.LogInformation("Error [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _store.SaveAsync(root, cancellation);
                _logger.LogInformation("User updated with success");
                return Result.Ok(_mapper.Map((Domain.Common.User)root.State));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception: ", operation.Id);
                return Result.Fail(e);
            }
            finally
            {
                scope.Dispose();
            }
        }
    }
}
