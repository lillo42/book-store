using System.Text;
using AutoFixture;

namespace IdentityServer.Domain.Test.Extensions
{
    public static class FixtureExtensions
    {
        public static string CreateWithLength(this Fixture fixture, int length)
        {
            var result = new StringBuilder();

            while (result.Length < length)
            {
                result.Append(fixture.Create<string>());
            }

            return result.ToString(0, length);
        }
    }
}