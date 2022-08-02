using BookStore.Domain.Abstractions;
using BookStore.Domain.Author.Events;

using static BookStore.Domain.Author.Errors;

namespace BookStore.Domain.Author;

public class AuthorAggregateRoot : AbstractAggregateRoot<AuthorState>
{
    public AuthorAggregateRoot(AuthorState state, long version) 
        : base(state, version)
    {
    }

    public void Create(string name)
    {
        CheckName(name);
        On(new Created(name));
    }

    public void ChangeName(string name)
    {
        CheckName(name);
        if (State.Name == name)
        {
            return;
        }
        
        On(new NameChanged(name));
    }

    private static void CheckName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new DomainException(NameIsMissing);
        }

        if (name.Length > 100)
        {
            throw new DomainException(NameIsTooLarge);
        }
    }
}