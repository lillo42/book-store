using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Web.Store
{
    public class ResourceStore : IResourceStore
    {
        private static readonly IEnumerable<IdentityResource> s_identity = new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        private readonly IReadOnlyResourceRepository _repository;

        public ResourceStore(IReadOnlyResourceRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
            => Task.FromResult(s_identity);

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var hash = new HashSet<ApiResource>();

            var resources = await _repository.GetByNamesAsync(scopeNames)
                .ConfigureAwait(false);
            
            foreach (var resource in resources)
            {
                hash.Add(ToApiResource(resource));
            }

            return hash;
        }

        public async Task<ApiResource> FindApiResourceAsync(string name) 
            => ToApiResource(await _repository.GetByNameAsync(name)
                .ConfigureAwait(false));

        public async Task<Resources> GetAllResourcesAsync()
        {
            var resources = await _repository.GetAllAsync()
                .ConfigureAwait(false);

            return new Resources(s_identity, resources.Select(ToApiResource).ToArray());
        }
        
        private static ApiResource ToApiResource(Domain.Common.Resource resource)
        {
            return new ApiResource(resource.Name, resource.DisplayName)
            {
                DisplayName = resource.DisplayName,
                Enabled = resource.IsEnable,
            };
        }
    }
}