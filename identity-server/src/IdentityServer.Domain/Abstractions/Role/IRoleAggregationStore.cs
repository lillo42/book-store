using System;

namespace IdentityServer.Domain.Abstractions.Role
{
    public interface IRoleAggregationStore : IAggregateStore<IRoleAggregationRoot, RoleState, Guid>
    {
        
    }
}