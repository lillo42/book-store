using System;

namespace IdentityServer.Application.Request.Client
{
    public class ClientAddResource
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
    }
}