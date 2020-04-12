using System.Collections.Generic;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions
{
    public interface IAggregateRoot<TState, TId>
        where TState : class, IState<TId>
    {
        TState State { get; }
        
        IEnumerable<Event> Events { get; }
    }
}