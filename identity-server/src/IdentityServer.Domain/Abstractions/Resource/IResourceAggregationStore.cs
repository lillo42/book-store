using System;

namespace IdentityServer.Domain.Abstractions.Resource
{
    public interface IResourceAggregationStore : IAggregateStore<IResourceAggregationRoot, ResourceState, Guid>
    {
        
    }
}