using System;

namespace IdentityServer.Application.Request.Client
{
    public class ClientRemoveRole
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
    }
}