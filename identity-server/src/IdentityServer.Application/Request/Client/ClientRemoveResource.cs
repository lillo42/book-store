using System;

namespace IdentityServer.Application.Request.Client
{
    public class ClientRemoveResource
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
    }
}