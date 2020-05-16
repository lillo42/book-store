using System;
using System.Linq;
using IdentityServer.Web.Proto;
using Permission = IdentityServer.Domain.Common.Permission;
using Role = IdentityServer.Domain.Common.Role;
using User = IdentityServer.Domain.Common.User;

namespace IdentityServer.Web.Mapper
{
    public class ClientMapper : IMapper<Domain.Common.Client, Client>
    {
        private readonly IMapper<Domain.Common.Permission, Proto.Permission> _permission;
        private readonly IMapper<Domain.Common.Role, Proto.Role> _role;
        private readonly IMapper<Domain.Common.Resource, Proto.Resource> _resource;

        public ClientMapper(IMapper<Domain.Common.Permission, Proto.Permission> permission, 
            IMapper<Domain.Common.Role, Proto.Role> role, 
            IMapper<Domain.Common.Resource, Proto.Resource> resource)
        {
            _permission = permission ?? throw new ArgumentNullException(nameof(permission));
            _role = role ?? throw new ArgumentNullException(nameof(role));
            _resource = resource ?? throw new ArgumentNullException(nameof(resource));
        }

        public Client Map(Domain.Common.Client source)
        {
            if (source == null)
            {
                return new Client();
            }
            
            var client = new Client
            {
                Id = source.Id.ToString(),
                Name = source.Name, 
                ClientId = source.ClientId,
                ClientSecret = source.ClientSecret
            };

            if(source.Permissions != null)
            {
                client.Permissions.Add(source
                    .Permissions
                    .Select(x => _permission.Map(x)));
            }
            
            if(source.Roles != null)
            {
                client.Roles.Add(source
                    .Roles
                    .Select(x => _role.Map(x)));
            }

            if (source.Resources != null)
            {
                client.Resources.Add(source
                    .Resources
                    .Select(x => _resource.Map(x)));
            }
            
            return client;
        }
    }
}