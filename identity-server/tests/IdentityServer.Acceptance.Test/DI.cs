using System;
using System.IO;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Acceptance.Test
{
    public class DI
    {
        public static IServiceProvider Provider { get; }

        static DI()
        {
            var configure = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();

            var collection = new ServiceCollection()
                .AddSingleton(configure);
            
            var channel = new Channel(configure.GetValue<string>("Host"), ChannelCredentials.Insecure);
            collection.AddScoped(p => new IdentityServer.Web.Proto.Resources.ResourcesClient(channel));

            Provider = collection.BuildServiceProvider();
        }
    }
}
