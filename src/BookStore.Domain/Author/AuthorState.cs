using BookStore.Domain.Abstractions;
using BookStore.Domain.Author.Events;

namespace BookStore.Domain.Author;

public class AuthorState : IState<Guid>
{
    public AuthorState(Guid id)
    {
        Id = id;
    }

    public AuthorState(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    
    public void On(NameChanged @event)
        => Name = @event.Name;
    
    public void On(Created @event)
        => Name = @event.Name;
}