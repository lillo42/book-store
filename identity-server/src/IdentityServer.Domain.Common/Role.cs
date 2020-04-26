using System;
using System.Collections.Generic;

namespace IdentityServer.Domain.Common
{
    public class Role : IEquatable<Role>
    {
        public Role()
        {
            
        }

        public Role(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string DisplayName { get; set; }
        
        public ISet<Permission> Permissions { get; set; } = new HashSet<Permission>();

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
            
            return Id == other.Id;
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
            => HashCode.Combine(Id);
    }
}