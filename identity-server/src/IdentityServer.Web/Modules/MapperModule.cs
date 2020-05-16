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
            
            builder.RegisterType<ResultToGetResourceByIdReplay>()
                .As<IMapper<Result, Proto.GetResourceByIdReplay>>()
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
            
            builder.RegisterType<ResultToGetPermissionByIdReplay>()
                .As<IMapper<Result, Proto.GetPermissionByIdReplay>>()
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
            
            builder.RegisterType<ResultToGetRoleByIdReplay>()
                .As<IMapper<Result, Proto.GetRoleByIdReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToAddRolePermissionReplay>()
                .As<IMapper<Result, Proto.AddRolePermissionReplay>>()
                .SingleInstance();

            builder.RegisterType<ResultToRemoveRolePermissionReplay>()
                .As<IMapper<Result, Proto.RemoveRolePermissionReplay>>()
                .SingleInstance();
            #endregion
            
            #region User
            builder.RegisterType<UserMapper>()
                .As<IMapper<Domain.Common.User, Proto.User>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToCreateUserReplay>()
                .As<IMapper<Result, Proto.CreateUserReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToUpdateUserReplay>()
                .As<IMapper<Result, Proto.UpdateUserReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToGetUserByIdReplay>()
                .As<IMapper<Result, Proto.GetUserByIdReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToAddUserPermissionReplay>()
                .As<IMapper<Result, Proto.AddUserPermissionReplay>>()
                .SingleInstance();

            builder.RegisterType<ResultToRemoveUserPermissionReplay>()
                .As<IMapper<Result, Proto.RemoveUserPermissionReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToRemoveUserRoleReplay>()
                .As<IMapper<Result, Proto.RemoveUserRoleReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToAddUserRoleReplay>()
                .As<IMapper<Result, Proto.AddUserRoleReplay>>()
                .SingleInstance();
            #endregion
            
            #region Client
            builder.RegisterType<ClientMapper>()
                .As<IMapper<Domain.Common.Client, Proto.Client>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToCreateClientReplay>()
                .As<IMapper<Result, Proto.CreateClientReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToUpdateClientReplay>()
                .As<IMapper<Result, Proto.UpdateClientReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToGetClientByIdReplay>()
                .As<IMapper<Result, Proto.GetClientByIdReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToAddClientPermissionReplay>()
                .As<IMapper<Result, Proto.AddClientPermissionReplay>>()
                .SingleInstance();

            builder.RegisterType<ResultToRemoveClientPermissionReplay>()
                .As<IMapper<Result, Proto.RemoveClientPermissionReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToRemoveClientRoleReplay>()
                .As<IMapper<Result, Proto.RemoveClientRoleReplay>>()
                .SingleInstance();
            
            builder.RegisterType<ResultToAddClientRoleReplay>()
                .As<IMapper<Result, Proto.AddClientRoleReplay>>()
                .SingleInstance();
            #endregion
        }
    }
}