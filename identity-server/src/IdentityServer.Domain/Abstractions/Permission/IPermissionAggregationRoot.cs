using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Abstractions.Permission
{
    public interface IPermissionAggregationRoot : IAggregateRoot<PermissionState, Guid>
    {
        Task<Result> CreateAsync(string name, string displayName, string description, CancellationToken cancellationToken = default);
        
        Task<Result> UpdateAsync(string name, string displayName, string description,
            CancellationToken cancellationToken = default);
        
    }
}