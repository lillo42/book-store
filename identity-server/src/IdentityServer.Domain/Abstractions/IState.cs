namespace IdentityServer.Domain.Abstractions
{
    public interface IState<T>
    {
        T Id { get; }
    }
}