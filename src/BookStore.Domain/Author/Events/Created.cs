using BookStore.Domain.Abstractions;

namespace BookStore.Domain.Author.Events;

public record Created(string Name) : AbstractEvent;