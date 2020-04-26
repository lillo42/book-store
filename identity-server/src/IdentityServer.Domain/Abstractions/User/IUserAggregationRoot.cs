using System;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Abstractions.User
{
    public interface IUserAggregationRoot : IAggregateRoot<UserState, Guid>
    {
        Result Create(string mail, string password, bool isEnable);
        
        Result Update(string mail, bool isEnable);
        
        Task<Result> AddPermissionAsync(Common.Permission permission);
        
        Result RemovePermission(Common.Permission permission);
        
        Task<Result> AddRoleAsync(Common.Role role);
        
        Result RemoveRole(Common.Role role);
    }
}