using Autofac;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Mapper;

namespace IdentityServer.Web.Modules
{
    public class MapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region Resource
            builder.RegisterType<ResourceMapper>()
                .As<IMapper<Domain.Common.Resource, Proto.Resource>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToCreateResourceReplay>()
                .As<IMapper<Result, Proto.CreateResourceReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToGetResourceByIeReplay>()
                .As<IMapper<Result, Proto.GetResourceByIeReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToUpdateResourceReplay>()
                .As<IMapper<Result, Proto.UpdateResourceReplay>>()
                .SingleInstance();
            #endregion

            #region Permission
            
            builder.RegisterType<PermissionMapper>()
                .As<IMapper<Domain.Common.Permission, Proto.Permission>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToCreatePermissionReplay>()
                .As<IMapper<Result, Proto.CreatePermissionReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToUpdatePermissionReplay>()
                .As<IMapper<Result, Proto.UpdatePermissionReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToGetPermissionByIeReplay>()
                .As<IMapper<Result, Proto.GetPermissionByIeReplay>>()
                .SingleInstance();
            #endregion

            #region Role
            builder.RegisterType<RoleMapper>()
                .As<IMapper<Domain.Common.Role, Proto.Role>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToCreateRoleReplay>()
                .As<IMapper<Result, Proto.CreateRoleReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToUpdateRoleReplay>()
                .As<IMapper<Result, Proto.UpdateRoleReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToGetRoleByIeReplay>()
                .As<IMapper<Result, Proto.GetRoleByIeReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToAddPermissionReplay>()
                .As<IMapper<Result, Proto.AddRolePermissionReplay>>()
                .SingleInstance();

            builder.RegisterType<ResultToRemovePermissionReplay>()
                .As<IMapper<Result, Proto.RemoveRolePermissionReplay>>()
                .SingleInstance();
            #endregion
        }
    }
}