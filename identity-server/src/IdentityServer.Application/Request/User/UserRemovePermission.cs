using System;

namespace IdentityServer.Application.Request.User
{
    public class UserRemovePermission
    {
        public Guid Id { get; set; }
        public Guid PermissionId { get; set; }
    }
}