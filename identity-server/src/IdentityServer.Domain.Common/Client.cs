using System;
using System.Collections.Generic;

namespace IdentityServer.Domain.Common
{
    public class Client
    {
        public Client()
        {
            Roles = new HashSet<Role>();
            Permissions = new HashSet<Permission>();
            Resources = new HashSet<Resource>();
        }
        
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool IsEnable { get; set; }
        
        public ISet<Role> Roles { get; set; }
        public ISet<Permission> Permissions { get; set; }
        public ISet<Resource> Resources { get; set; }
    }
}