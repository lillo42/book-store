using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Domain.Abstractions
{
    public interface IAggregateStore<TAggregateRoot, TState, TId>
        where  TAggregateRoot : IAggregateRoot<TState, TId>
        where TState : class, IState<TId>
    {
        TAggregateRoot Create();
        Task<TAggregateRoot> GetAsync(TId id, CancellationToken cancellation = default);
        Task SaveAsync(TAggregateRoot aggregate, CancellationToken cancellation = default);
    }
}