using BookStore.Domain.Abstractions;

namespace BookStore.Domain.Author.Events;

public record NameChanged(string Name) : AbstractEvent;