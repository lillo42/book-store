using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Users.Application.Contracts.Request;
using Users.Application.Contracts.Response;
using Users.Application.Mapper;
using Users.Domain;
using Users.Infrastructure;

namespace Users.Application.Operations
{
    public class UserGetOperation : IOperation<UserGet>
    {
        private readonly IReadOnlyUserRepository _repository;
        private readonly IMapper<Domain.Common.User, User> _mapper;
        private readonly ILogger<UserGetOperation> _logger;

        public UserGetOperation(IReadOnlyUserRepository repository, 
            ILogger<UserGetOperation> logger, 
            IMapper<Domain.Common.User, User> mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async ValueTask<Result> ExecuteAsync(UserGet operation, CancellationToken cancellation = default)
        {
            var scope = _logger.BeginScope("Get user. [UserId: {userId}]", operation.Id);
            try
            {
                var user = await _repository.GetByIdAsync(operation.Id, cancellation);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    return DomainError.UserError.UserNotFound;
                }

                _logger.LogInformation("Get user with success");
                return Result.Ok(_mapper.Map(user));
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
