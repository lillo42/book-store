using System;
using System.IO;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Users.Acceptance.Test
{
    public static class DI
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
            collection.AddScoped(p => new Web.Proto.Users.UsersClient(channel));

            Provider = collection.BuildServiceProvider();
        }
    }
}
