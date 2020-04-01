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
    public class UserCreateOperation : IOperation<UserAdd>
    {
        private readonly IUserAggregateStore _store;
        private readonly IReadOnlyUserRepository _repository;
        private readonly IMapper<Domain.Common.User, User> _mapper;
        private readonly ILogger<UserCreateOperation> _logger;

        public UserCreateOperation(IUserAggregateStore store, 
            IReadOnlyUserRepository repository, 
            ILogger<UserCreateOperation> logger, 
            IMapper<Domain.Common.User, User> mapper)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async ValueTask<Result> ExecuteAsync(UserAdd operation, CancellationToken cancellation = default)
        {
            var scope = _logger.BeginScope("Creating user.");
            try
            {
                var root = _store.Create();

                if (root.Create(operation.Email, operation.FirstName,
                    operation.LastNames, operation.BirthDate) is ErrorResult error)
                {
                    _logger.LogInformation("Error [ErrorCode: {errorCode}]", error.ErrorCode);
                    return error;
                }

                if (await _repository.EmailExistAsync(operation.Email, cancellation))
                {
                    _logger.LogInformation("Email already exist");
                    return DomainError.UserError.EmailAlreadyExist;
                }

                await _store.SaveAsync(root, cancellation);
                _logger.LogInformation("User created: [UserId: {userId}]", root.State.Id );
                return Result.Ok(_mapper.Map((Domain.Common.User)root.State));
                
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
