using System;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Client.Events
{
    public class CreateClientEvent : Event
    {
        public CreateClientEvent(string name, string clientId, string clientSecret, bool isEnable)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
            IsEnable = isEnable;
        }

        public string Name { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public bool IsEnable { get; }
    }
}