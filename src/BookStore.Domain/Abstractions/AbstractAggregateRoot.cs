using System.Collections.Immutable;

namespace BookStore.Domain.Abstractions;

public abstract class AbstractAggregateRoot<T> : IAggregateRoot<T>
    where T : IState
{
    protected AbstractAggregateRoot(T state, long version)
    {
        State = state;
        Version = version;
        Events = ImmutableList<IEvent>.Empty;
    }

    public T State { get; }
    public long Version { get; }
    public ImmutableList<IEvent> Events { get; private set; }

    protected void On<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        ((dynamic)State).On(@event);
        Events = Events.Add(@event);
    }
}