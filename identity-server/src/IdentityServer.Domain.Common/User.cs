using System;
using System.Collections.Generic;

namespace IdentityServer.Domain.Common
{
    public class User
    {
        public Guid Id { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public bool IsEnable { get; set; }
        
        public ISet<Role> Roles { get; set; } = new HashSet<Role>();
        public ISet<Permission> Permissions { get; set; } = new HashSet<Permission>();
    }
}