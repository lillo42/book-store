using System;

namespace IdentityServer.Application.Request.Client
{
    public class ClientAddRole
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
    }
}