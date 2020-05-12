using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions.Permission;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Domain.Permission
{
    public class PermissionAggregationStore : IPermissionAggregationStore
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<PermissionAggregationStore> _logger;

        public PermissionAggregationStore(IUnitOfWork unitOfWork,
            IPermissionRepository permissionRepository,
            IEventRepository eventRepository, 
            ILoggerFactory loggerFactory)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<PermissionAggregationStore>();
        }

        public IPermissionAggregationRoot Create()
        {
            _logger.LogDebug($"Going to create new {nameof(PermissionAggregationRoot)}");
            return CreateNew(new Common.Permission());
        }

        public async Task<IPermissionAggregationRoot> GetAsync(Guid id, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going to get role. [Permission: {permissionId}]", id);
            var role = await _permissionRepository.GetByIdAsync(id, cancellation)
                .ConfigureAwait(false);
            
            return role == null ? null : CreateNew(role);
        }
        
        private PermissionAggregationRoot CreateNew(Common.Permission permission)
        {
            return  new PermissionAggregationRoot(new PermissionState(permission), 
                _permissionRepository,
                _loggerFactory.CreateLogger<PermissionAggregationRoot>());
        }

        public async Task SaveAsync(IPermissionAggregationRoot aggregate, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going being transaction");
            using (_unitOfWork.BeginTransactionAsync())
            {
                var permission = (Common.Permission) aggregate.State;
                var repository = _permissionRepository;

                if (permission.Id == Guid.Empty)
                {
                    _logger.LogDebug("Going to create new permission.");
                    await repository.CreateAsync(permission, cancellation)
                        .ConfigureAwait(false);
                }
                else
                {
                    _logger.LogDebug("Going to update permission.");
                    await repository.UpdateAsync(permission, cancellation)
                        .ConfigureAwait(false);
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