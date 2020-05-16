using Autofac;
using IdentityServer.Infrastructure;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using IdentityServer.Infrastructure.Repositories;
using IdentityServer.Web.Configuration;
using IdentityServer.Web.Factory;
using IdentityServer.Web.IdentityServer4.Index;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace IdentityServer.Web.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PostgresDbFactory>()
                .As<IDbFactory>()
                .SingleInstance();
            
            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
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
                    
                    new PersistedGrant_ByKey().Execute(store);
                    new PersistedGrant_BySubjectIdByClientIdByType().Execute(store);
                    
                    return store;
                })
                .AsSelf()
                .As<IDocumentStore>()
                .SingleInstance();

            builder.Register(ctx => ctx.Resolve<IDocumentStore>().OpenAsyncSession())
                .As<IAsyncDocumentSession>()
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