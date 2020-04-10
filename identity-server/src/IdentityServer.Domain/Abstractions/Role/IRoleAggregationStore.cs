using System;
using IdentityServer.Domain.Roles;

namespace IdentityServer.Domain.Abstractions.Role
{
    public interface IRoleAggregationStore : IAggregateStore<RoleAggregationRoot, RoleState, Guid>
    {
        
    }
}