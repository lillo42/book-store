using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions.Resource;
using IdentityServer.Domain.Role;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Domain.Resource
{
    public class ResourceAggregationStore : IResourceAggregationStore
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IResourceRepository _resourceRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RoleAggregationStore> _logger;
        
        public ResourceAggregationStore(IUnitOfWork unitOfWork,
            IResourceRepository resourceRepository,
            IEventRepository eventRepository, 
            ILoggerFactory loggerFactory)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _resourceRepository = resourceRepository ?? throw new ArgumentNullException(nameof(resourceRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<RoleAggregationStore>();
        }

        
        public IResourceAggregationRoot Create()
        {
            _logger.LogDebug($"Going to create new {nameof(RoleAggregationRoot)}");
            return CreateNew(new Common.Resource());
        }

        public async Task<IResourceAggregationRoot> GetAsync(Guid id, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going to get role. [RoleId: {roleId}]", id);
            var resource = await _resourceRepository.GetByIdAsync(id, cancellation)
                .ConfigureAwait(false);
            
            return resource == null ? null : CreateNew(resource);
        }
        
        private ResourceAggregationRoot CreateNew(Common.Resource role)
        {
            return new ResourceAggregationRoot(new ResourceState(role),
                _resourceRepository,
                _loggerFactory.CreateLogger<ResourceAggregationRoot>());
        }

        public async Task SaveAsync(IResourceAggregationRoot aggregate, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going being transaction");
            using (_unitOfWork.BeginTransactionAsync())
            {
                var resource = (Common.Resource) aggregate.State;
                var repository = _resourceRepository;

                if (resource.Id == Guid.Empty)
                {
                    await repository.CreateAsync(resource, cancellation)
                        .ConfigureAwait(false);
                }
                else
                {
                    await repository.UpdateAsync(resource, cancellation)
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