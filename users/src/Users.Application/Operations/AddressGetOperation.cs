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
    public class AddressGetOperation : IOperation<AddressGet>
    {
        private readonly IUserAggregateStore _store;
        private readonly IMapper<Domain.Common.Address, Address> _mapper;
        private readonly ILogger<AddressGetOperation> _logger;

        public AddressGetOperation(IUserAggregateStore store, 
            ILogger<AddressGetOperation> logger, 
            IMapper<Domain.Common.Address, Address> mapper)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async ValueTask<Result> ExecuteAsync(AddressGet operation, CancellationToken cancellation = default)
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
                
                _logger.LogInformation("Address get with success");
                return Result.Ok(root.State.Addresses.Select(x=> _mapper.Map(x)));
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Exception: ");
                return Result.Fail(e);
            }
            finally
            {
                scope.Dispose();
            }
        }
    }
}
