namespace BookStore.Domain.Abstractions;

public interface IState<out T> : IState
    where T : notnull
{
    new T Id { get; }
    object IState.Id => Id;
}

public interface IState
{
    object Id { get; }
}