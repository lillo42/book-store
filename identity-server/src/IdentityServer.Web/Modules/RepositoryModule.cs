using Autofac;
using IdentityServer.Infrastructure.Repositories;

namespace IdentityServer.Web.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>()
                .As<IReadOnlyUsersRepository>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ClientRepository>()
                .As<IReadOnlyClientRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ResourceRepository>()
                .As<IReadOnlyResourceRepository>()
                .InstancePerLifetimeScope();
        }
    }
}