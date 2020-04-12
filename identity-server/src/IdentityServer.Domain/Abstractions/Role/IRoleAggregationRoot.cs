using System;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Role
{
    public interface IRoleAggregationRoot : IAggregateRoot<RoleState, Guid>
    {
        Result Create(string name, string displayName, string description);
        
        Result Update(string name, string displayName, string description);

        Task<Result> AddPermission(Common.Permission permission);
        
        Task<Result> RemovePermission(Common.Permission permission);
    }
}