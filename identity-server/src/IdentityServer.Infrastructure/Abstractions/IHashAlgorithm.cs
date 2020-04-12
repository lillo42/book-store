namespace IdentityServer.Infrastructure.Abstractions
{
    public interface IHashAlgorithm
    {
        string ComputeHash(string text);
    }
}