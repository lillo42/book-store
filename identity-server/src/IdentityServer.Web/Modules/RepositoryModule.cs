using System.Data.Common;
using Autofac;
using IdentityServer.Infrastructure;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using IdentityServer.Infrastructure.Repositories;
using IdentityServer.Web.Configuration;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Session;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace IdentityServer.Web.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(ctx =>
                {
                    var connection = new ProfiledDbConnection(
                        new NpgsqlConnection(ctx.Resolve<IConfiguration>().GetConnectionString("Postgres")),
                        MiniProfiler.Current);
                    
                    connection.Open();

                    return connection;
                })
                .As<DbConnection>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.Register(ctx =>
                {
                    var configuration = ctx.Resolve<RavenDbConfiguration>();
                    var store =  new DocumentStore()
                    {
                        Urls = configuration.Urls,
                        Database = configuration.Database,
                    };

                    store.Initialize();

                    return store;
                })
                .AsSelf()
                .As<IDocumentStore>()
                .InstancePerLifetimeScope();

            builder.Register(ctx => ctx.Resolve<IDocumentStore>().OpenAsyncSession())
                .As<IAsyncDocumentSession>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EventRepository>()
                .As<IEventRepository>()
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