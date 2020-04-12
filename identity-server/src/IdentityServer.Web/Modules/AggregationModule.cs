using Autofac;
using IdentityServer.Domain.Abstractions.Permission;
using IdentityServer.Domain.Abstractions.Role;
using IdentityServer.Domain.Abstractions.User;
using IdentityServer.Domain.Permission;
using IdentityServer.Domain.Role;
using IdentityServer.Domain.User;

namespace IdentityServer.Web.Modules
{
    public class AggregationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PermissionAggregationStore>()
                .As<IPermissionAggregationStore>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<RoleAggregationStore>()
                .As<IRoleAggregationStore>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<UserAggregationStore>()
                .As<IUserAggregationStore>()
                .InstancePerLifetimeScope();
        }
    }
}