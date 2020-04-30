using System;

namespace IdentityServer.Application.Request.Client
{
    public class ClientAddPermission
    {
        public Guid Id { get; set; }
        public Guid PermissionId { get; set; }
    }
}