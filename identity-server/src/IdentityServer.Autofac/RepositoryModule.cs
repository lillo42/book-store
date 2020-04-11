using Autofac;
using IdentityServer.Infrastructure;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace IdentityServer.Autofac
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx => new NpgsqlConnection(
                ctx.Resolve<IConfiguration>().GetConnectionString("Postgres")))
                .AsSelf()
                .InstancePerRequest();
            
            
            builder.RegisterType(typeof(UnitOfWork<>))
                .As(typeof(IUnitOfWork<>))
                .InstancePerLifetimeScope();
            
            builder.RegisterType<PermissionRepository>()
                .As<IPermissionRepository>()
                .As<IReadOnlyPermissionRepository>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<RoleRepository>()
                .As<IRoleRepository>()
                .As<IReadOnlyRoleRepository>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<EventRepository>()
                .As<IEventRepository>()
                .InstancePerLifetimeScope();
        }
    }
}