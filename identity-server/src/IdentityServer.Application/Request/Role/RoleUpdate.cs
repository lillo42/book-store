using System;

namespace IdentityServer.Application.Request.Role
{
    public class RoleUpdate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}