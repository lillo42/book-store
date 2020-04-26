using System;

namespace IdentityServer.Application.Request.Role
{
    public class RoleAddPermission
    {
        public Guid Id { get; set; }
        public Guid PermissionId { get; set; }
    }
}