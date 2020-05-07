using System;
using System.Linq;
using IdentityServer.Web.Proto;
using Permission = IdentityServer.Domain.Common.Permission;

namespace IdentityServer.Web.Mapper
{
    public class RoleMapper : IMapper<Domain.Common.Role, Proto.Role>
    {
        private readonly IMapper<Domain.Common.Permission, Proto.Permission> _permission;

        public RoleMapper(IMapper<Permission, Proto.Permission> permission)
        {
            _permission = permission ?? throw new ArgumentNullException(nameof(permission));
        }

        public Role Map(Domain.Common.Role source)
        {
            if (source == null)
            {
                return new Role();
            }
            
            var role = new Role
            {
                Id = source.Id.ToString(),
                Name = source.Name,
                Description = source.Description ?? string.Empty,
                DisplayName = source.DisplayName
            };

            if(source.Permissions != null)
            {
                role.Permission.Add(source
                    .Permissions
                    .Select(x => _permission.Map(x)));
            }
            
            return role;
        }
    }
}