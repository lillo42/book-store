using System;
using System.Collections.Generic;

namespace IdentityServer.Infrastructure.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public string Password { get; set; }
        
        public ICollection<Role> Roles { get; set; }
        public ICollection<Permission> Permissions { get; set; }
    }
}