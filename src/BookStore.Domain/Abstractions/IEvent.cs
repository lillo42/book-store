namespace BookStore.Domain.Abstractions;

public interface IEvent
{
    DateTime OccurredOn { get; }
}