using System;
using System.Collections.Generic;

namespace IdentityServer.Domain.Common
{
    public class Client : IEquatable<Client>
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

        public bool Equals(Client other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Client client)
            {
                return Equals(client);
            }
            
            return false;
        }

        public override int GetHashCode() 
            => HashCode.Combine(Id);
    }
}