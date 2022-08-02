using System.Collections.Immutable;

namespace BookStore.Domain.Abstractions;

public interface IAggregateRoot<out T>
    where T : IState
{
    T State { get; }
    long Version { get; }
    ImmutableList<IEvent> Events { get; }
}