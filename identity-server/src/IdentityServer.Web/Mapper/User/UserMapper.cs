using System;
using System.Linq;
using IdentityServer.Web.Proto;
using Permission = IdentityServer.Domain.Common.Permission;
using Role = IdentityServer.Domain.Common.Role;

namespace IdentityServer.Web.Mapper
{
    public class UserMapper : IMapper<Domain.Common.User, Proto.User>
    {
        private readonly IMapper<Domain.Common.Permission, Proto.Permission> _permission;
        private readonly IMapper<Domain.Common.Role, Proto.Role> _role;

        public UserMapper(IMapper<Permission, Proto.Permission> permission, 
            IMapper<Role, Proto.Role> role)
        {
            _permission = permission ?? throw new ArgumentNullException(nameof(permission));
            _role = role ?? throw new ArgumentNullException(nameof(role));
        }

        public User Map(Domain.Common.User source)
        {
            if (source == null)
            {
                return new User();
            }
            
            var user = new User
            {
                Id = source.Id.ToString(),
                Mail = source.Mail, 
                Password = "",
                IsEnable = source.IsEnable
            };

            if(source.Permissions != null)
            {
                user.Permissions.Add(source
                    .Permissions
                    .Select(x => _permission.Map(x)));
            }
            
            if(source.Roles != null)
            {
                user.Roles.Add(source
                    .Roles
                    .Select(x => _role.Map(x)));
            }
            
            return user;
        }
    }
}