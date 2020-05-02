using System;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Acceptance.Test.Scenes
{
    public class BaseScene
    {
        protected Fixture Fixture { get; }
        protected IServiceProvider Provider { get; }
        
        protected Web.Proto.Resources.ResourcesClient Client { get; }
        
        protected BaseScene()
        {
            Fixture = new Fixture();
            Provider = DI.Provider.CreateScope().ServiceProvider;
            Client = Provider.GetRequiredService<Web.Proto.Resources.ResourcesClient>();
        }
    }
}