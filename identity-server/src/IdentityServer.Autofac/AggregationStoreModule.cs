using Autofac;
using IdentityServer.Domain.Abstractions.Permission;
using IdentityServer.Domain.Abstractions.Role;
using IdentityServer.Domain.Permission;
using IdentityServer.Domain.Roles;

namespace IdentityServer.Autofac
{
    public class AggregationStoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PermissionAggregationStore>()
                .As<IPermissionAggregationStore>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<RoleAggregationStore>()
                .As<IRoleAggregationStore>()
                .InstancePerLifetimeScope();
        }
    }
}