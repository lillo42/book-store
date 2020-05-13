using System;

namespace IdentityServer.Domain.Common
{
    public class Resource : IEquatable<Resource>
    {
        public Resource()
        {
            
        }

        public Resource(Guid id)
        {
            Id = id;
        }
        
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsEnable { get; set; }

        public bool Equals(Resource other)
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

            if (obj is Resource resource)
            {
                return Equals(resource);
            }
            
            return false;
        }

        public override int GetHashCode() 
            => HashCode.Combine(Id);
    }
}