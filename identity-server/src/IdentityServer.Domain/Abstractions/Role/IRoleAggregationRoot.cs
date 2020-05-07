using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Abstractions.Role
{
    public interface IRoleAggregationRoot : IAggregateRoot<RoleState, Guid>
    {
        Task<Result> CreateAsync(string name, string displayName, string description, CancellationToken cancellationToken = default);
        
        Task<Result> UpdateAsync(string name, string displayName, string description, CancellationToken cancellationToken = default);

        Task<Result> AddPermission(Common.Permission permission);
        
        Result RemovePermission(Common.Permission permission);
    }
}