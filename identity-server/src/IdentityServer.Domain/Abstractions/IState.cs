namespace IdentityServer.Domain.Abstractions
{
    public interface IState<T> : IState
    {
        new T Id { get; }

        object IState.Id => Id;
    }

    public interface IState
    {
        object Id { get; }
    }
}