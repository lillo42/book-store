using System;

namespace IdentityServer.Domain.Common
{
    public class Permission : IEquatable<Permission>
    {
        public Permission()
        {
            
        }

        public Permission(Guid id)
        {
            Id = id;
        }
        
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string DisplayName { get; set; }
        
        public bool Equals(Permission other)
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

            if (obj is Permission role)
            {
                return Equals(role);
            }
            
            return false;
        }

        public override int GetHashCode() 
            => HashCode.Combine(Id);
    }
}