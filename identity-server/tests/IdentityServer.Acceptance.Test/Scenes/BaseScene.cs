using System;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Acceptance.Test.Scenes
{
    public class BaseScene
    {
        protected Fixture Fixture { get; }
        protected IServiceProvider Provider { get; }

        protected BaseScene()
        {
            Fixture = new Fixture();
            Provider = DI.Provider.CreateScope().ServiceProvider;
        }

    }
}