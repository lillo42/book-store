namespace BookStore.Domain.Abstractions;

public record AbstractEvent : IEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}