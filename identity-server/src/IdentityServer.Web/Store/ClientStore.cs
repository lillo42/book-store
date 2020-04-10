// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using IdentityModel;
// using IdentityServer.Infrastructure.Repositories;
// using IdentityServer4;
// using IdentityServer4.Models;
// using IdentityServer4.Stores;
//
// namespace IdentityServer.Web.Store
// {
//     public class ClientStore : IClientStore
//     {
//         private readonly IReadOnlyClientRepository _repository;
//
//         public ClientStore(IReadOnlyClientRepository repository)
//         {
//             _repository = repository ?? throw new ArgumentNullException(nameof(repository));
//         }
//
//         public async Task<Client> FindClientByIdAsync(string clientId)
//         {
//             var client = await _repository.GetClientAsync(clientId);
//             
//             if (client != null)
//             {
//                 return new Client
//                 {
//                     ClientId = client.ClientId,
//                     ClientName =  clientId,
//                     //Claims = client.Roles.Select(x => new Claim(JwtClaimTypes.Role, x.Name)).ToHashSet(),
//                     ClientSecrets = new List<Secret>
//                     {
//                         new Secret(client.Password)
//                     },
//                     AllowedGrantTypes =
//                     {
//                         GrantType.ClientCredentials
//                     },
//                     AllowedScopes =
//                     {
//                         IdentityServerConstants.StandardScopes.OpenId,
//                         JwtClaimTypes.Role,
//                     },
//                     Enabled = true,
//                 };
//             }
//
//             return null;
//         }
//     }
// }