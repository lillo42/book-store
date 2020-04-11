using System;

namespace IdentityServer.Domain.Abstractions.Permission
{
    public interface IPermissionAggregationRoot : IAggregateRoot<PermissionState, Guid>
    {
        Result Create(string name, string displayName, string description);
        
        Result Update(string name, string displayName, string description);
        
    }
}