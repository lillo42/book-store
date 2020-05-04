using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Abstractions.Resource
{
    public interface IResourceAggregationRoot : IAggregateRoot<ResourceState, Guid>
    {
        Task<Result> CreateAsync(string name, string displayName, string description, bool isEnable, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(string name, string displayName, string description, bool isEnable, CancellationToken cancellationToken = default);
    }
}