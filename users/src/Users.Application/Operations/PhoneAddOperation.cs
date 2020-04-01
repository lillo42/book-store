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
    public class PhoneAddOperation : IOperation<PhoneAdd>
    {
        private readonly IUserAggregateStore _store;
        private readonly ILogger<PhoneAddOperation> _logger;
        private readonly IMapper<Domain.Common.Phone, Phone> _mapper;

        public PhoneAddOperation(IUserAggregateStore store,
            ILogger<PhoneAddOperation> logger,
            IMapper<Domain.Common.Phone, Phone> mapper)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async ValueTask<Result> ExecuteAsync(PhoneAdd operation, CancellationToken cancellation = default)
        {
            var scope = _logger.BeginScope("Add Phone. [UserId: {userId}]", operation.UserId);
            try
            {
                var root = await _store.GetAsync(operation.UserId, cancellation);

                if (root == null)
                {
                    _logger.LogInformation("User not found");
                    return DomainError.UserError.UserNotFound;
                }

                if (root.AddPhone(operation.Number) is ErrorResult error)
                {
                    _logger.LogInformation("Error. [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                await _store.SaveAsync(root, cancellation);
                _logger.LogInformation("Phone added with success");
                var phone = root.State.Phones.Last();
                return Result.Ok(_mapper.Map(phone));
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
