using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Client;
using IdentityServer.Domain.Abstractions.Client.Events;
using IdentityServer.Domain.Extensions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

using static IdentityServer.Domain.DomainError;

namespace IdentityServer.Domain.Client
{
    public class ClientAggregationRoot : AggregateRoot<ClientState, Guid>, IClientAggregationRoot
    {
        private readonly IReadOnlyClientRepository _clientRepository;
        private readonly IReadOnlyPermissionRepository _permissionsRepository;
        private readonly IReadOnlyRoleRepository _rolesRepository;
        private readonly IReadOnlyResourceRepository _resourcesRepository;

        public ClientAggregationRoot(ClientState state, 
            IReadOnlyClientRepository clientRepository,
            IReadOnlyPermissionRepository permissionRepository, 
            IReadOnlyRoleRepository rolesRepository, 
            IReadOnlyResourceRepository resourcesRepository,
            ILogger<ClientAggregationRoot> logger) 
            : base(state, logger)
        {
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            _permissionsRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _rolesRepository = rolesRepository ?? throw new ArgumentNullException(nameof(rolesRepository));
            _resourcesRepository = resourcesRepository ?? throw new ArgumentNullException(nameof(resourcesRepository));
        }

        public async Task<Result> CreateAsync(string name, string clientId, string clientSecret, bool isEnable, CancellationToken cancellationToken = default)
        {
            if (name.IsMissing())
            {
                return ClientError.MissingName;
            }
            
            if (name.Length > 100)
            {
                return ClientError.InvalidName;
            }

            if (await _clientRepository.ExistNameAsync(name, cancellationToken)
                .ConfigureAwait(false))
            {
                return ClientError.NameAlreadyExist;
            }
            
            if (clientId.IsMissing())
            {
                return ClientError.MissingClientId;
            }
            
            if (clientId.Length > 50)
            {
                return ClientError.InvalidClientId;
            }
            
            if (await _clientRepository.ExistClientIdAsync(clientId, cancellationToken)
                .ConfigureAwait(false))
            {
                return ClientError.ClientIdAlreadyExist;
            }
            
            if (clientSecret.IsMissing())
            {
                return ClientError.MissingClientSecret;
            }
            
            if (clientSecret.Length > 250)
            {
                return ClientError.InvalidClientSecret;
            }
            
            Apply(new CreateClientEvent(name, clientId, clientSecret, isEnable));
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(string name, string clientId, string clientSecret, bool isEnable, CancellationToken cancellationToken = default)
        {
            if (name.IsMissing())
            {
                return ClientError.MissingName;
            }
            
            if (name.Length > 100)
            {
                return ClientError.InvalidName;
            }
            
            if(State.Name != name && await _clientRepository.ExistNameAsync(name, cancellationToken)
                .ConfigureAwait(false))
            {
                return ClientError.NameAlreadyExist;
            }
            
            if (clientId.IsMissing())
            {
                return ClientError.MissingClientId;
            }
            
            if (clientId.Length > 50)
            {
                return ClientError.InvalidClientId;
            }

            if (State.ClientId != clientId && await _clientRepository.ExistClientIdAsync(clientId, cancellationToken)
                .ConfigureAwait(false))
            {
                return ClientError.ClientIdAlreadyExist;
            }

            if (clientSecret.IsMissing())
            {
                return ClientError.MissingClientSecret;
            }
            
            if (clientSecret.Length > 250)
            {
                return ClientError.InvalidClientSecret;
            }
            
            Apply(new UpdateClientEvent(name, clientId, clientSecret, isEnable));
            return Result.Ok();
        }

        public async Task<Result> AddPermissionAsync(Common.Permission permission, CancellationToken cancellationToken = default)
        {
            if (permission == null)
            {
                return ClientError.InvalidPermission;
            }

            if (!await _permissionsRepository.ExistAsync(permission.Id, cancellationToken)
                .ConfigureAwait(false))
            {
                return ClientError.InvalidPermission;
            }
            
            if (State.Permissions.Contains(permission))
            {
                return ClientError.PermissionAlreadyExist;
            }
            
            Apply(new AddPermissionEvent(permission));
            return Result.Ok();
        }

        public Result RemovePermission(Common.Permission permission)
        {
            if (permission == null)
            {
                return ClientError.InvalidPermission;
            }
            
            if (!State.Permissions.Contains(permission))
            {
                return ClientError.NotContainsPermission;
            }
            
            Apply(new RemovePermissionEvent(permission));
            return Result.Ok();
        }

        public async Task<Result> AddRoleAsync(Common.Role role, CancellationToken cancellationToken = default)
        {
            if (role == null)
            {
                return ClientError.InvalidRole;
            }
            
            if (!await _rolesRepository.ExistAsync(role.Id, cancellationToken)
                .ConfigureAwait(false))
            {
                return ClientError.InvalidRole;
            }
            
            if (State.Roles.Contains(role))
            {
                return ClientError.RoleAlreadyExist;
            }
            
            Apply(new AddRoleEvent(role));
            return Result.Ok();
        }

        public Result RemoveRole(Common.Role role)
        {
            if (role == null)
            {
                return ClientError.InvalidRole;
            }

            if (!State.Roles.Contains(role))
            {
                return ClientError.NotContainsRole;
            }
            
            Apply(new RemoveRoleEvent(role));
            return Result.Ok();
        }

        public async Task<Result> AddResourceAsync(Common.Resource resource, CancellationToken cancellationToken = default)
        {
            if (resource == null)
            {
                return ClientError.InvalidResource;
            }
            
            if (!await _resourcesRepository.ExistAsync(resource.Id, cancellationToken)
                .ConfigureAwait(false))
            {
                return ClientError.InvalidResource;
            }
            
            if (State.Resources.Contains(resource))
            {
                return ClientError.ResourceAlreadyExist;
            }
            
            Apply(new AddResourceEvent(resource));
            return Result.Ok();
        }

        public Result RemoveResource(Common.Resource resource)
        {
            if (resource == null)
            {
                return ClientError.InvalidResource;
            }
            
            if (!State.Resources.Contains(resource))
            {
                return ClientError.NotContainsResource;
            }
            
            Apply(new RemoveResourceEvent(resource));
            return Result.Ok();
        }
    }
}