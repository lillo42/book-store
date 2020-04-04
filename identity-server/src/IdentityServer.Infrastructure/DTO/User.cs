using System;
using System.Collections.Generic;

namespace IdentityServer.Infrastructure.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        
        public ICollection<Role> Roles { get; set; }
    }
}