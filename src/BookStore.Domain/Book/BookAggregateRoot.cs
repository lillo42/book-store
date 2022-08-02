using BookStore.Domain.Abstractions;
using BookStore.Domain.Book.Services;
using BookStore.Domain.Book.ValueObjects;

namespace BookStore.Domain.Book;

public class BookAggregateRoot : AbstractAggregateRoot<BookState>
{
    public BookAggregateRoot(BookState state, long version) 
        : base(state, version)
    {
    }

    public async Task ChangePublisherAsync(Publisher publisher,
        IPublisherService publisherService,
        CancellationToken cancellationToken = default)
    {
        if (!await publisherService.ExistAsync(publisher, cancellationToken))
        {
            throw new Exception();
        }
        
        // On(new PublisherChanged(publisher));
    }
}