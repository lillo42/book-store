using System;

namespace IdentityServer.Application.Request.User
{
    public class UserAddRole
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
    }
}