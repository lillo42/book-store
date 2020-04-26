using System;

namespace IdentityServer.Application.Request.Resource
{
    public class ResourceUpdate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsEnable { get; set; }
    }
}