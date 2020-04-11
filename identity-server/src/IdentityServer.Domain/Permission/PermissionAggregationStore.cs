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
        private readonly IUnitOfWork<IPermissionRepository> _unitOfWork;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<PermissionAggregationStore> _logger;

        public PermissionAggregationStore(IUnitOfWork<IPermissionRepository> unitOfWork, 
            IEventRepository eventRepository, 
            ILoggerFactory loggerFactory)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
            var role = await _unitOfWork.Repository.GetByIdAsync(id, cancellation)
                .ConfigureAwait(false);
            
            return role == null ? null : CreateNew(role);
        }
        
        private PermissionAggregationRoot CreateNew(Common.Permission permission)
        {
            return  new PermissionAggregationRoot(new PermissionState(permission), 
                _loggerFactory.CreateLogger<PermissionAggregationRoot>());
        }

        public async Task SaveAsync(IPermissionAggregationRoot aggregate, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going being transaction");
            using (_unitOfWork.BeginTransaction())
            {
                var permission = (Common.Permission) aggregate.State;

                if (permission.Id == Guid.Empty)
                {
                    _logger.LogDebug("Going to create new permission.");
                    await _unitOfWork.Repository.CreateAsync(permission, cancellation)
                        .ConfigureAwait(false);
                }
                else
                {
                    _logger.LogDebug("Going to update permission.");
                    await _unitOfWork.Repository.CreateAsync(permission, cancellation)
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