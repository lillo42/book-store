using System;

namespace IdentityServer.Application.Request.User
{
    public class UserRemoveRole
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
    }
}