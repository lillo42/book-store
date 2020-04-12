using System.Security.Cryptography;
using System.Text;
using IdentityServer.Infrastructure.Abstractions;

namespace IdentityServer.Infrastructure
{
    public class SHA256Algorithm : IHashAlgorithm
    {
        public string ComputeHash(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            using var hashAlgorithm = SHA256.Create();
            var data = Encoding.UTF8.GetBytes(text);
            var hash = hashAlgorithm.ComputeHash(data);
            
            var sb = new StringBuilder();
            foreach (var @byte in hash)
            {
                sb.Append(@byte.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}