using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Abstractions.User
{
    public interface IUserAggregationRoot : IAggregateRoot<UserState, Guid>
    {
        Task<Result> CreateAsync(string mail, string password, bool isEnable, CancellationToken cancellationToken = default);
        
        Task<Result> UpdateAsync(string mail, bool isEnable, CancellationToken cancellationToken = default);
        
        Task<Result> AddPermissionAsync(Common.Permission permission);
        
        Result RemovePermission(Common.Permission permission);
        
        Task<Result> AddRoleAsync(Common.Role role);
        
        Result RemoveRole(Common.Role role);
    }
}