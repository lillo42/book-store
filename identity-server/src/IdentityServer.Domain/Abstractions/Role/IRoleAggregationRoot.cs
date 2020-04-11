using System;
using IdentityServer.Domain.Common;
using IdentityServer.Domain.Roles;

namespace IdentityServer.Domain.Abstractions.Role
{
    public interface IRoleAggregationRoot : IAggregateRoot<RoleState, Guid>
    {
        Result Create(string name, string displayName, string description);
        
        Result Update(string name, string displayName, string description);

        Result AddPermission(Common.Permission permission);
        
        Result RemovePermission(Common.Permission permission);
    }
}