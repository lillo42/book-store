using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Common;

namespace IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly
{
    public interface IReadOnlyUserRepository
    {
        Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        IAsyncEnumerable<User> GetAllAsync(CancellationToken cancellationToken = default);

        Task<User> GetByMailAndPasswordAsync(string mail, string password, CancellationToken cancellationToken = default);

        Task<bool> IsEnableAsync(Guid id, CancellationToken cancellationToken = default);
    }
}