using System;
using System.Collections.Generic;

namespace IdentityServer.Infrastructure.Entities
{
    public class Role : IEquatable<Role>
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string DisplayName { get; set; }
        
        public ICollection<Permission> Permissions { get; set; }

        public bool Equals(Role other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return Name == other.Name;
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

            if (obj is Role role)
            {
                return Equals(role);
            }
            
            return false;
        }

        public override int GetHashCode() 
            => HashCode.Combine(Name);
    }
}