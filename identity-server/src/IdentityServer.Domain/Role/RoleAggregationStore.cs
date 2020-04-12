using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions.Role;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Domain.Role
{
    public class RoleAggregationStore : IRoleAggregationStore
    {
        private readonly IUnitOfWork<IRoleRepository> _unitOfWork;
        private readonly IEventRepository _eventRepository;
        private readonly IReadOnlyPermissionRepository _permissionRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RoleAggregationStore> _logger;

        public RoleAggregationStore(IUnitOfWork<IRoleRepository> repository, 
            IEventRepository eventRepository, 
            ILoggerFactory loggerFactory, 
            IReadOnlyPermissionRepository permissionRepository)
        {
            _unitOfWork = repository ?? throw new ArgumentNullException(nameof(repository));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _logger = _loggerFactory.CreateLogger<RoleAggregationStore>();
        }

        public IRoleAggregationRoot Create()
        {
            _logger.LogDebug($"Going to create new {nameof(RoleAggregationRoot)}");
            return CreateNew(new Common.Role());
        }

        public async Task<IRoleAggregationRoot> GetAsync(Guid id, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going to get role. [RoleId: {roleId}]", id);
            var role = await _unitOfWork.Repository.GetByIdAsync(id, cancellation)
                .ConfigureAwait(false);
            
            return role == null ? null : CreateNew(role);
        }

        private RoleAggregationRoot CreateNew(Common.Role role)
        {
            return  new RoleAggregationRoot(new RoleState(role),
                _loggerFactory.CreateLogger<RoleAggregationRoot>(),
                _permissionRepository);
        }

        public async Task SaveAsync(IRoleAggregationRoot aggregate, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going being transaction");
            using (_unitOfWork.BeginTransaction())
            {
                var role = (Common.Role) aggregate.State;

                if (role.Id == Guid.Empty)
                {
                    await _unitOfWork.Repository.CreateAsync(role, cancellation)
                        .ConfigureAwait(false);
                }
                else
                {
                    await _unitOfWork.Repository.CreateAsync(role, cancellation)
                        .ConfigureAwait(false);
                }

                await _unitOfWork.Repository.AddPermissionsAsync(role, cancellation)
                    .ConfigureAwait(false);

                await _unitOfWork.Repository.RemovePermissionsAsync(role, cancellation)
                    .ConfigureAwait(false);

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