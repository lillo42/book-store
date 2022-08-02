using System.Collections.Immutable;
using BookStore.Domain.Abstractions;
using BookStore.Domain.Book.ValueObjects;

namespace BookStore.Domain.Book;

public class BookState : IState<Guid>
{
    public BookState(Guid id)
    {
        Id = id;
        Authors = ImmutableList<ValueObjects.Author>.Empty;
        Publisher = new(string.Empty);
        Name = string.Empty;
        Description = string.Empty;
    }

    public Guid Id { get; }
    public ImmutableList<ValueObjects.Author> Authors { get; private set; }
    public Publisher Publisher { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
}