using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Abstractions.Client
{
    public interface IClientAggregationRoot : IAggregateRoot<ClientState, Guid>
    {
        Task<Result> CreateAsync(string name, string clientId, string clientSecret, bool isEnable, CancellationToken cancellationToken = default);
        
        Task<Result> UpdateAsync(string name, string clientId, string clientSecret, bool isEnable, CancellationToken cancellationToken = default);
        
        Task<Result> AddPermissionAsync(Common.Permission permission, CancellationToken cancellationToken = default);
        
        Result RemovePermission(Common.Permission permission);
        
        Task<Result> AddRoleAsync(Common.Role role, CancellationToken cancellationToken = default);
        
        Result RemoveRole(Common.Role role);
        
        Task<Result> AddResourceAsync(Common.Resource resource, CancellationToken cancellationToken = default);
        
        Result RemoveResource(Common.Resource resource);
    }
}