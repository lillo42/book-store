using System.Data.Common;
using Autofac;
using IdentityServer.Infrastructure;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using IdentityServer.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace IdentityServer.Web.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(ctx => new ProfiledDbConnection(
                    new NpgsqlConnection(ctx.Resolve<IConfiguration>().GetConnectionString("Postgres")),
                    MiniProfiler.Current))
                .As<DbConnection>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
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
                .As<IResourceRepository>()
                .As<IReadOnlyResourceRepository>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ClientRepository>()
                .As<IClientRepository>()
                .As<IReadOnlyClientRepository>()
                .InstancePerLifetimeScope();
        }
    }
}