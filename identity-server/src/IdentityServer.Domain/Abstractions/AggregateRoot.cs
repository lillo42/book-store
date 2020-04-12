using System;
using System.Collections.Generic;
using IdentityServer.Domain.Common;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Domain.Abstractions
{
    public abstract class AggregateRoot<TState, TId> : IAggregateRoot<TState, TId>
        where TState : class, IState<TId>
    {
        public TState State { get; }
        public IEnumerable<Event> Events => _events;
        
        protected ILogger Logger { get; }

        private readonly ICollection<Event> _events;

        protected AggregateRoot(TState state, 
            ILogger logger)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _events = new List<Event>();
        }

        protected void Apply<TEvent>(TEvent @event)
            where TEvent : Event
        {
            Logger.LogTrace("Going to apply event '{eventName}'", typeof(TEvent).Name);
            ((dynamic)State).Apply(@event);
            _events.Add(@event);
        }
    }
}