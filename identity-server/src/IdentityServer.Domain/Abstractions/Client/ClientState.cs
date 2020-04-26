using System;
using IdentityServer.Infrastructure;

namespace IdentityServer.Domain.Abstractions.Client
{
    public class ClientState : IState<Guid>
    {
        private readonly Common.Client _client;

        public ClientState(Common.Client client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            
            Roles = new HashSetTrace<Common.Role>(_client.Roles);
            Permissions = new HashSetTrace<Common.Permission>(_client.Permissions);
            Resources = new HashSetTrace<Common.Resource>(_client.Resources);
        }

        public Guid Id => _client.Id;
        
        public string Name => _client.Name;
        public string ClientId => _client.ClientId;
        public string ClientSecret => _client.ClientSecret;
        public bool IsEnable  => _client.IsEnable;
        
        public HashSetTrace<Common.Role> Roles { get; }
        public HashSetTrace<Common.Permission> Permissions { get; }
        public HashSetTrace<Common.Resource> Resources { get; }

        public static explicit operator Common.Client(ClientState state)
            => state._client;
    }
}