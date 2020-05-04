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
        }
    }
}