using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions.User;
using IdentityServer.Domain.Role;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Domain.User
{
    public class UserAggregationStore : IUserAggregationStore
    {
        private readonly IUnitOfWork<IUserRepository> _unitOfWork;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RoleAggregationStore> _logger;
        private readonly IHashAlgorithm _hash;
        private readonly IReadOnlyRoleRepository _roleRepository;
        private readonly IReadOnlyPermissionRepository _permissionRepository;

        public UserAggregationStore(IUnitOfWork<IUserRepository> repository, 
            IEventRepository eventRepository,
            IHashAlgorithm hash,
            ILoggerFactory loggerFactory, 
            IReadOnlyPermissionRepository permissionRepository, 
            IReadOnlyRoleRepository roleRepository)
        {
            _unitOfWork = repository ?? throw new ArgumentNullException(nameof(repository));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
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
            var role = await _unitOfWork.Repository.GetByIdAsync(id, cancellation)
                .ConfigureAwait(false);
            
            return role == null ? null : CreateNew(role);
        }
        
        private UserAggregationRoot CreateNew(Common.User role)
        {
            return new UserAggregationRoot(new UserState(role),
                _loggerFactory.CreateLogger<UserAggregationRoot>(),
                _hash, _permissionRepository, _roleRepository);
        }

        public async Task SaveAsync(IUserAggregationRoot aggregate, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going being transaction");
            using (_unitOfWork.BeginTransaction())
            {
                var user = (Common.User) aggregate.State;

                if (user.Id == Guid.Empty)
                {
                    await _unitOfWork.Repository.CreateAsync(user, cancellation)
                        .ConfigureAwait(false);
                }
                else
                {
                    await _unitOfWork.Repository.CreateAsync(user, cancellation)
                        .ConfigureAwait(false);
                }

                if (aggregate.State.PermissionsHasChange)
                {
                    await _unitOfWork.Repository.AddPermissionsAsync(user, cancellation)
                        .ConfigureAwait(false);

                    await _unitOfWork.Repository.RemovePermissionsAsync(user, cancellation)
                        .ConfigureAwait(false);
                }

                if (aggregate.State.RolesHasChange)
                {
                    await _unitOfWork.Repository.AddRolesAsync(user, cancellation)
                        .ConfigureAwait(false);

                    await _unitOfWork.Repository.RemoveRolesAsync(user, cancellation)
                        .ConfigureAwait(false);
                }

                _logger.LogDebug("Going to save changes");
                await _unitOfWork.SaveAsync(cancellation)
                    .ConfigureAwait(false);
            }

            _logger.LogDebug("Going to save events");
            await _eventRepository.SaveAsync(aggregate.Events, cancellation)
                .ConfigureAwait(false);
        }
    }
}