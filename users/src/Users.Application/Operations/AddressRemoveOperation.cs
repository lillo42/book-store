using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Users.Application.Contracts;
using Users.Domain;

namespace Users.Application.Operations
{
    public class AddressRemoveOperation : IOperation<AddressRemove>
    {
        private readonly IUserAggregateStore _store;
        private readonly ILogger<AddressRemoveOperation> _logger;

        public AddressRemoveOperation(IUserAggregateStore store, 
            ILogger<AddressRemoveOperation> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<Result> ExecuteAsync(AddressRemove operation, CancellationToken cancellation = default)
        {
            var scope = _logger.BeginScope("Remove Address. [UserId: {userId}]", operation.UserId);
            try
            {
                var root = await _store.GetAsync(operation.UserId, cancellation);
                if (root == null)
                {
                    _logger.LogInformation("User not found");
                    return DomainError.UserError.UserNotFound;
                }

                if (root.RemoveAddress(operation.Id) is ErrorResult error)
                {
                    _logger.LogInformation("Error [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _store.SaveAsync(root, cancellation);
                _logger.LogInformation("Address removed with success");
                return Result.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception: ");
                return Result.Fail(e);
            }
            finally
            {
                scope.Dispose();
            }
        }
    }
}
