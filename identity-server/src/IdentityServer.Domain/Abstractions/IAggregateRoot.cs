using System.Collections.Generic;

namespace IdentityServer.Domain.Abstractions
{
    public interface IAggregateRoot<TState, TId>
        where TState : class, IState<TId>
    {
        TState State { get; }
        
        IEnumerable<IEvent> Events { get; }
    }
}