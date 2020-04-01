using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Users.Application.Contracts.Request;
using Users.Application.Contracts.Response;
using Users.Application.Mapper;
using Users.Domain;

namespace Users.Application.Operations
{
    public class AddressAddOperation : IOperation<AddressAdd>
    {
        private readonly IUserAggregateStore _store;
        private readonly ILogger<AddressAddOperation> _logger;
        private readonly IMapper<Domain.Common.Address, Address> _mapper;

        public AddressAddOperation(IUserAggregateStore store, 
            ILogger<AddressAddOperation> logger, 
            IMapper<Domain.Common.Address, Address> mapper)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async ValueTask<Result> ExecuteAsync(AddressAdd operation, CancellationToken cancellation = default)
        { 
            var scope = _logger.BeginScope("Get Address. [UserId: {userId}]", operation.UserId);
            try
            {
                var root = await _store.GetAsync(operation.UserId, cancellation);

                if (root == null)
                {
                    _logger.LogInformation("User not found");
                    return DomainError.UserError.UserNotFound;
                }
                
                if (root.AddAddress(operation.Line, operation.Number, operation.PostCode) is ErrorResult error)
                {
                    _logger.LogInformation("Error. [ErrorCode: {errorCode}].", error.ErrorCode);
                    return error;
                }

                await _store.SaveAsync(root, cancellation);

                _logger.LogInformation("Address added with success");
                var address = root.State.Addresses.Last();

                return Result.Ok(_mapper.Map(address));
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
