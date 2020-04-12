using Autofac;
using IdentityServer.Infrastructure;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using IdentityServer.Infrastructure.Repositories;

namespace IdentityServer.Web.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(UnitOfWork<>))
                .As(typeof(IUnitOfWork<>))
                .InstancePerLifetimeScope();

            builder.RegisterType<RoleRepository>()
                .As<IRoleRepository>()
                .As<IReadOnlyRoleRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PermissionRepository>()
                .As<IPermissionRepository>()
                .As<IReadOnlyPermissionRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UserRepository>()
                .As<IUserRepository>()
                .As<IReadOnlyUserRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ResourceRepository>()
                .As<IReadOnlyResourceRepository>()
                .InstancePerLifetimeScope();
        }
    }
}