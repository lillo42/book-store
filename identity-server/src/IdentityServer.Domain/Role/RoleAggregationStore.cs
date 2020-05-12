using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions.Role;
using IdentityServer.Infrastructure;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Domain.Role
{
    public class RoleAggregationStore : IRoleAggregationStore
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;
        private readonly IReadOnlyPermissionRepository _permissionRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RoleAggregationStore> _logger;

        public RoleAggregationStore(IUnitOfWork unitOfWork,
            IRoleRepository roleRepository,
            IReadOnlyPermissionRepository permissionRepository,
            IEventRepository eventRepository, 
            ILoggerFactory loggerFactory)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
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
            var role = await _roleRepository.GetByIdAsync(id, cancellation)
                .ConfigureAwait(false);
            
            return role == null ? null : CreateNew(role);
        }

        private RoleAggregationRoot CreateNew(Common.Role role)
        {
            return  new RoleAggregationRoot(new RoleState(role),
                _roleRepository,
                _permissionRepository,
                _loggerFactory.CreateLogger<RoleAggregationRoot>());
        }

        public async Task SaveAsync(IRoleAggregationRoot aggregate, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going being transaction");
            using (_unitOfWork.BeginTransactionAsync())
            {
                var role = (Common.Role) aggregate.State;
                var repository = _roleRepository;

                if (role.Id == Guid.Empty)
                {
                    await repository.CreateAsync(role, cancellation)
                        .ConfigureAwait(false);
                }
                else
                {
                    await repository.UpdateAsync(role, cancellation)
                        .ConfigureAwait(false);
                }

                foreach (var trace in aggregate.State.Permissions.Traces)
                {
                    switch (trace.State)
                    {
                        case State.Added:
                            await repository.AddPermissionAsync(role, trace.Value, cancellation)
                                .ConfigureAwait(false);
                            break;
                        case State.Removed when !trace.IsNew:
                            await repository.RemovePermissionAsync(role, trace.Value, cancellation)
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