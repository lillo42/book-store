using BookStore.Domain.Book.ValueObjects;

namespace BookStore.Domain.Book.Services;

public interface IPublisherService
{
    Task<bool> ExistAsync(Publisher publisher, CancellationToken cancellationToken = default);
}