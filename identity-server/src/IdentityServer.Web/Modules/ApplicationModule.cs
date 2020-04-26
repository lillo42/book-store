using Autofac;
using IdentityServer.Application.Operation.Permission;
using IdentityServer.Application.Operation.Resource;
using IdentityServer.Application.Operation.Role;
using IdentityServer.Application.Operation.User;
using PermissionUpdateOperation = IdentityServer.Application.Operation.Permission.PermissionUpdateOperation;
using RoleGetAllOperation = IdentityServer.Application.Operation.Role.RoleGetAllOperation;

namespace IdentityServer.Web.Modules
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region Resource
            builder.RegisterType<ResourceCreateOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<ResourceUpdateOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ResourceGetAllOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ResourceGetOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            #endregion
            
            #region Permission
            builder.RegisterType<PermissionCreateOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<PermissionUpdateOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<PermissionGetAllOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<PermissionGetOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            #endregion
            
            #region Role
            builder.RegisterType<RoleCreateOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<RoleUpdateOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<RoleAddPermissionOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<RoleRemovePermissionOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<RoleGetAllOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<RoleGetOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            #endregion
            
            #region User
            builder.RegisterType<UserCreateOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<UserUpdateOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<UserAddPermissionOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<UserRemovePermissionOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<UserAddRoleOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<UserRemoveRoleOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();

            
            builder.RegisterType<UserGetAllOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<UserGetOperation>()
                .AsSelf()
                .InstancePerLifetimeScope();
            #endregion
        }
    }
}