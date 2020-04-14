// using System;
// using System.Collections.Generic;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
namespace IdentityServer.Web.Store
{
    public class ClientStore : IClientStore
    {
        private readonly IReadOnlyClientRepository _repository;
        public ClientStore(IReadOnlyClientRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await _repository.GetByClientIdAsync(clientId);

            if (client == null)
            {
                return null;
            }
            
            var scopes = new HashSet<string>
            {
                IdentityServerConstants.StandardScopes.OpenId, 
                JwtClaimTypes.Role, 
                "permission"
            };
                
            foreach (var resource in client.Resources)
            {
                scopes.Add(resource.Name);
            }
            
            var claims = new HashSet<Claim>();

            foreach (var role in client.Roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role.Name));
            }
            
            foreach (var permission in client.Permissions)
            {
                claims.Add(new Claim("permission", permission.Name));
            }
            
            return new Client
            {
                ClientId = client.ClientId,
                ClientName =  client.Name,
                Claims = claims,
                ClientSecrets = new List<Secret>
                {
                    new Secret(client.ClientSecret)
                },
                AllowedGrantTypes = { GrantType.Hybrid},
                AllowedScopes = scopes,
                Enabled = client.IsEnable,
            };
        }
    }
}