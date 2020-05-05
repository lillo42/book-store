using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class PermissionMapper : IMapper<Domain.Common.Permission, Proto.Permission>
    {
        public Permission Map(Domain.Common.Permission source)
        {
            if (source == null)
            {
                return new Permission();
            }
            
            return new Permission
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                Description = source.Description ?? string.Empty,
                DisplayName = source.DisplayName,
            };
        }
    }
}