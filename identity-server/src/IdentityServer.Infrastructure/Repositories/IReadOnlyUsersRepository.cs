using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Infrastructure.Entities;

namespace IdentityServer.Infrastructure.Repositories
{
    public interface IReadOnlyUsersRepository
    {
        Task<Guid> GetByEmailAsync(string mail, string password);
        Task<IEnumerable<Role>> GetUsersRoles(Guid id);
        Task<bool> IsUserEnableAsync(Guid id);
    }
}