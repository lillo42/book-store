using System;

namespace IdentityServer.Application.Request.Client
{
    public class ClientRemovePermission
    {
        public Guid Id { get; set; }
        public Guid PermissionId { get; set; }
    }
}