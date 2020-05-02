using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions.Client;
using IdentityServer.Infrastructure;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Domain.Client
{
    public class ClientAggregationStore : IClientAggregationStore
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ClientAggregationStore> _logger;

        public ClientAggregationStore(IUnitOfWork unitOfWork, 
            IEventRepository eventRepository, ILoggerFactory loggerFactory, 
            ILogger<ClientAggregationStore> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IClientAggregationRoot Create()
        {
            _logger.LogDebug($"Going to create new {nameof(ClientAggregationRoot)}");
            return CreateNew(new Common.Client());
        }

        public async Task<IClientAggregationRoot> GetAsync(Guid id, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going to get user. [UserId: {roleId}]", id);
            var role = await _unitOfWork.ClientRepository.GetByIdAsync(id, cancellation)
                .ConfigureAwait(false);
            
            return role == null ? null : CreateNew(role);
        }
        
        private ClientAggregationRoot CreateNew(Common.Client entity)
        {
            return new ClientAggregationRoot(new ClientState(entity),
                _loggerFactory.CreateLogger<ClientAggregationRoot>(),
                _unitOfWork.PermissionRepository, _unitOfWork.RoleRepository,
                _unitOfWork.ResourceRepository);
        }

        public async Task SaveAsync(IClientAggregationRoot aggregate, CancellationToken cancellation = default)
        {
            _logger.LogDebug("Going being transaction");
            using (_unitOfWork.BeginTransaction())
            {
                var entity = (Common.Client) aggregate.State;
                var repository = _unitOfWork.ClientRepository;
                
                if (entity.Id == Guid.Empty)
                {
                    await repository.CreateAsync(entity, cancellation)
                        .ConfigureAwait(false);
                }
                else
                {
                    await repository.UpdateAsync(entity, cancellation)
                        .ConfigureAwait(false);
                }

                foreach (var trace in aggregate.State.Roles.Traces)
                {
                    switch (trace.State)
                    {
                        case State.Added:
                            await repository.AddRoleAsync(entity, trace.Value, cancellation)
                                .ConfigureAwait(false);
                            break;
                        case State.Removed when trace.IsNew:
                            await repository.RemoveRoleAsync(entity, trace.Value, cancellation)
                                .ConfigureAwait(false);
                            break;
                    }
                }
                
                foreach (var trace in aggregate.State.Permissions.Traces)
                {
                    switch (trace.State)
                    {
                        case State.Added:
                            await repository.AddPermissionAsync(entity, trace.Value, cancellation)
                                .ConfigureAwait(false);
                            break;
                        case State.Removed when trace.IsNew:
                            await repository.RemovePermissionAsync(entity, trace.Value, cancellation)
                                .ConfigureAwait(false);
                            break;
                    }
                }
                
                foreach (var trace in aggregate.State.Resources.Traces)
                {
                    switch (trace.State)
                    {
                        case State.Added:
                            await repository.AddResourceAsync(entity, trace.Value, cancellation)
                                .ConfigureAwait(false);
                            break;
                        case State.Removed when trace.IsNew:
                            await repository.RemoveResourceAsync(entity, trace.Value, cancellation)
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