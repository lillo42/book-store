using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResourceMapper : IMapper<Domain.Common.Resource, Proto.Resource>
    {
        public Resource Map(Domain.Common.Resource source)
        {
            if (source == null)
            {
                return null;
            }
            
            return new Resource
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                Description = source.Description,
                DisplayName = source.DisplayName,
                IsEnable = source.IsEnable
            };
        }
    }
}