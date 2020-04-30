using System;

namespace IdentityServer.Application.Request.Client
{
    public class ClientUpdate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool IsEnable { get; set; }
    }
}