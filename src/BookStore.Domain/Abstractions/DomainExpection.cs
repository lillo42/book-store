namespace BookStore.Domain.Abstractions;

public class DomainException : Exception
{
    public DomainException(Error error)
    {
        Error = error;
    }

    public Error Error { get; }
}