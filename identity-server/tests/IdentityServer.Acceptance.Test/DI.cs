using System;
using System.IO;
using System.Net.Http;
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
            
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var channel = new Channel(configure.GetValue<string>("Host"), ChannelCredentials.Insecure);
            collection.AddScoped(p => new Web.Proto.Resources.ResourcesClient(channel));
            collection.AddScoped(p => new Web.Proto.Permissions.PermissionsClient(channel));
            collection.AddScoped(p => new Web.Proto.Roles.RolesClient(channel));
            collection.AddScoped(p => new Web.Proto.Users.UsersClient(channel));
            collection.AddScoped(p => new Web.Proto.Clients.ClientsClient(channel));
            collection.AddSingleton(provider => new HttpClient
            {
                BaseAddress = new Uri($"http://{configure.GetValue<string>("Host")}"),
                DefaultRequestVersion = new Version("2.0"),
            });
            
            
            Provider = collection.BuildServiceProvider();
        }
    }
}
