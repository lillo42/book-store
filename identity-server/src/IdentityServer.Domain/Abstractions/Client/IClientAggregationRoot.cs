using System;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Abstractions.Client
{
    public interface IClientAggregationRoot : IAggregateRoot<ClientState, Guid>
    {
        Result Create(string name, string clientId, string clientSecret, bool isEnable);
        
        Result Update(string name, string clientId, string clientSecret, bool isEnable);
        
        Task<Result> AddPermissionAsync(Common.Permission permission);
        
        Task<Result> RemovePermissionAsync(Common.Permission permission);
        
        Task<Result> AddRoleAsync(Common.Role role);
        
        Task<Result> RemoveRoleAsync(Common.Role role);
        
        Task<Result> AddResourceAsync(Common.Resource resource);
        
        Task<Result> RemoveResourceAsync(Common.Resource resource);
    }
}