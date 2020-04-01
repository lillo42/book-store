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
    public class PhoneGetOperation : IOperation<PhoneGet>
    {
        private readonly IUserAggregateStore _store;
        private readonly IMapper<Domain.Common.Phone, Phone> _mapper;
        private readonly ILogger<PhoneGetOperation> _logger;

        public PhoneGetOperation(IUserAggregateStore store, 
            ILogger<PhoneGetOperation> logger,
            IMapper<Domain.Common.Phone, Phone> mapper)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async ValueTask<Result> ExecuteAsync(PhoneGet operation, CancellationToken cancellation = default)
        {
            var scope = _logger.BeginScope("Get phone. [UserId: {userId}]", operation.UserId);
            try
            {
                var root = await _store.GetAsync(operation.UserId, cancellation);
                if (root == null)
                {
                    _logger.LogInformation("User not found");
                    return DomainError.UserError.UserNotFound;
                }
                
                _logger.LogInformation("Get Phone with success");
                return Result.Ok(root.State.Phones.Select(x=> _mapper.Map(x)));
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
