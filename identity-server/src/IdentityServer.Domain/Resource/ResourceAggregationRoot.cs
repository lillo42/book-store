using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Resource;
using IdentityServer.Domain.Abstractions.Resource.Events;
using IdentityServer.Domain.Extensions;
using Microsoft.Extensions.Logging;

using static IdentityServer.Domain.DomainError;

namespace IdentityServer.Domain.Resource
{
    public class ResourceAggregationRoot : AggregateRoot<ResourceState, Guid>, IResourceAggregationRoot
    {
        public ResourceAggregationRoot(ResourceState state, 
            ILogger<ResourceAggregationRoot> logger) 
            : base(state, logger)
        {
        }

        public Result Create(string name, string displayName, string description, bool isEnable)
        {
            if (name.IsMissing())
            {
                return ResourceError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return ResourceError.InvalidName;
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

        public Result Update(string name, string displayName, string description, bool isEnable)
        {
            if (name.IsMissing())
            {
                return ResourceError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return ResourceError.InvalidName;
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