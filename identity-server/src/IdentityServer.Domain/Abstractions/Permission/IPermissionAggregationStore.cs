using System;

namespace IdentityServer.Domain.Abstractions.Permission
{
    public interface IPermissionAggregationStore : IAggregateStore<IPermissionAggregationRoot, PermissionState, Guid>
    {
       
    }
}