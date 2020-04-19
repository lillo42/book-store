using System;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Client.Events
{
    public class CreateClientEvent : Event
    {
        public CreateClientEvent(string mail, string password, string clientSecret, bool isEnable)
        {
            Name = mail ?? throw new ArgumentNullException(nameof(mail));
            ClientId = password ?? throw new ArgumentNullException(nameof(password));
            ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
            IsEnable = isEnable;
        }

        public string Name { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public bool IsEnable { get; }
    }
}