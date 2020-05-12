using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions.User;
using IdentityServer.Domain.Role;
using IdentityServer.Infrastructure;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Domain.User
{
    public class UserAggregationStore : IUserAggregationStore
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IReadOnlyPermissionRepository _permissionRepository;
        private readonly IReadOnlyRoleRepository _roleRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RoleAggregationStore> _logger;
        private readonly IHashAlgorithm _hash;
        public UserAggregationStore(IUnitOfWork unitOfWork,
            IUserRepository userRepository, 
            IReadOnlyPermissionRepository permissionRepository, 
            IReadOnlyRoleRepository roleRepository,
            IEventRepository eventRepository,
            IHashAlgorithm hash,
            ILoggerFactory loggerFactory)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _hash = hash ?? throw new ArgumentNullException(nameof(hash));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _logger = _loggerFactory.CreateLogger<RoleAggregationStore>();
        }
        
        public IUserAggregationRoot Create()
        {
            _logger.LogDebug($"Going to create new {nameof(UserAggregationStore)}");
            return CreateNew(new Common.User());
        }

        public async Task<IUserAggregationRoot> GetAsync(Guid id, CancellationToken cancellation = default)
        { 
            _logger.LogDebug("Going to get user. [UserId: {roleId}]", id);
            var entity = await _userRepository.GetByIdAsync(id, cancellation)
                .ConfigureAwait(false);
            
            return entity == null ? null : CreateNew(entity);
        }
        
        private UserAggregationRoot CreateNew(Common.User role)
        {
            return new UserAggregationRoot(new UserState(role),
                _hash, _userRepository, _permissionRepository, _roleRepository,
                _loggerFactory.CreateLogger<UserAggregationRoot>());
        }

        public async Task SaveAsync(IUserAggregationRoot aggregate, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going being transaction");
            using (_unitOfWork.BeginTransactionAsync())
            {
                var user = (Common.User) aggregate.State;
                var repository = _userRepository;

                if (user.Id == Guid.Empty)
                {
                    await repository.CreateAsync(user, cancellation)
                        .ConfigureAwait(false);
                }
                else
                {
                    await repository.UpdateAsync(user, cancellation)
                        .ConfigureAwait(false);
                }

                foreach (var trace in aggregate.State.Permissions.Traces)
                {
                    switch (trace.State)
                    {
                        case State.Added:
                            await repository.AddPermissionAsync(user, trace.Value, cancellation)
                                .ConfigureAwait(false);
                            break;
                        case State.Removed when !trace.IsNew:
                            await repository.RemovePermissionAsync(user, trace.Value, cancellation)
                                .ConfigureAwait(false);
                            break;
                    }
                }

                foreach (var trace in aggregate.State.Roles.Traces)
                {
                    switch (trace.State)
                    {
                        case State.Added:
                            await repository.AddRoleAsync(user, trace.Value, cancellation)
                                .ConfigureAwait(false);
                            break;
                        case State.Removed when !trace.IsNew:
                            await repository.RemoveRoleAsync(user, trace.Value, cancellation)
                                .ConfigureAwait(false);
                            break;
                    }
                }

                _logger.LogDebug("Going to save changes");
                await _unitOfWork.CommitAsync(cancellation)
                    .ConfigureAwait(false);
            }

            _logger.LogDebug("Going to save events");
            await _eventRepository.SaveAsync(aggregate.Events, cancellation)
                .ConfigureAwait(false);
        }
    }
}