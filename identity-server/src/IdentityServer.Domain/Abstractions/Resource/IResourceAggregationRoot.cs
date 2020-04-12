using System;

namespace IdentityServer.Domain.Abstractions.Resource
{
    public interface IResourceAggregationRoot : IAggregateRoot<ResourceState, Guid>
    {
        Result Create(string name, string displayName, string description, bool isEnable);
        Result Update(string name, string displayName, string description, bool isEnable);
    }
}