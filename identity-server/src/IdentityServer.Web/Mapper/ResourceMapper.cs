using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResourceMapper : IMapper<Domain.Common.Resource, Proto.Resource>
    {
        public Resource Map(Domain.Common.Resource source)
        {
            if (source == null)
            {
                return new Resource();
            }
            
            return new Resource
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                Description = source.Description ?? string.Empty,
                DisplayName = source.DisplayName,
                IsEnable = source.IsEnable
            };
        }
    }
}