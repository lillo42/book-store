using System;
using System.Collections.Generic;

namespace IdentityServer.Infrastructure.Entities
{
    public class Resource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
    }
}