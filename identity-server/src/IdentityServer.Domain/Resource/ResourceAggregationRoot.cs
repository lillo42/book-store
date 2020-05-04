using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Resource;
using IdentityServer.Domain.Abstractions.Resource.Events;
using IdentityServer.Domain.Extensions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

using static IdentityServer.Domain.DomainError;

namespace IdentityServer.Domain.Resource
{
    public class ResourceAggregationRoot : AggregateRoot<ResourceState, Guid>, IResourceAggregationRoot
    {
        private readonly IReadOnlyResourceRepository _repository;
        public ResourceAggregationRoot(ResourceState state,
            IReadOnlyResourceRepository repository,
            ILogger<ResourceAggregationRoot> logger) 
            : base(state, logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result> CreateAsync(string name, string displayName, string description, bool isEnable, CancellationToken cancellationToken = default)
        {
            if (name.IsMissing())
            {
                return ResourceError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return ResourceError.InvalidName;
            }

            if (await _repository.ExistAsync(name, cancellationToken)
                .ConfigureAwait(false))
            {
                return ResourceError.NameAlreadyExist;
            }
            
            if (displayName.IsMissing())
            {
                return ResourceError.MissingDisplayName;
            }
            
            if (displayName.Length > 50)
            {
                return ResourceError.InvalidDisplayName;
            }
            
            if (description != null && description.Length > 250)
            {
                return ResourceError.InvalidDescription;
            }
            
            Apply(new CreateResourceEvent(name, displayName, description, isEnable));
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(string name, string displayName, string description, bool isEnable, CancellationToken cancellationToken = default)
        {
            if (name.IsMissing())
            {
                return ResourceError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return ResourceError.InvalidName;
            }
            
            if (State.Name != name && await _repository.ExistAsync(name, cancellationToken)
                .ConfigureAwait(false))
            {
                return ResourceError.NameAlreadyExist;
            }
            
            if (displayName.IsMissing())
            {
                return ResourceError.MissingDisplayName;
            }
            
            if (displayName.Length > 50)
            {
                return ResourceError.InvalidDisplayName;
            }
            
            if (description != null && description.Length > 250)
            {
                return ResourceError.InvalidDescription;
            }
            
            Apply(new UpdateResourceEvent(name, displayName, description, isEnable));
            return Result.Ok();
        }
    }
}