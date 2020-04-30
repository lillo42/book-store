using System;

namespace IdentityServer.Domain.Common
{
    public class Resource
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
    }
}