using System;
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
        private readonly IReadOnlyPermissionRepository _permissions;
        private readonly IReadOnlyRoleRepository _roles;
        private readonly IReadOnlyResourceRepository _resources;

        public ClientAggregationRoot(ClientState state, 
            ILogger<ClientAggregationRoot> logger, 
            IReadOnlyPermissionRepository permission, 
            IReadOnlyRoleRepository roles, 
            IReadOnlyResourceRepository resources) 
            : base(state, logger)
        {
            _permissions = permission ?? throw new ArgumentNullException(nameof(permission));
            _roles = roles ?? throw new ArgumentNullException(nameof(roles));
            _resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public Result Create(string name, string clientId, string clientSecret, bool isEnable)
        {
            if (name.IsMissing())
            {
                return ClientError.MissingName;
            }
            
            if (name.Length > 100)
            {
                return ClientError.InvalidName;
            }
            
            if (clientId.IsMissing())
            {
                return ClientError.MissingClientId;
            }
            
            if (clientId.Length > 50)
            {
                return ClientError.InvalidClientId;
            }
            
            if (clientSecret.IsMissing())
            {
                return ClientError.MissingClientSecret;
            }
            
            if (clientSecret.Length > 250)
            {
                return ClientError.InvalidClientSecret;
            }
            
            Apply(new CreateClientEvent(name, clientId, clientId, isEnable));
            return Result.Ok();
        }

        public Result Update(string name, string clientId, string clientSecret, bool isEnable)
        {
            if (name.IsMissing())
            {
                return ClientError.MissingName;
            }
            
            if (name.Length > 100)
            {
                return ClientError.InvalidName;
            }
            
            if (clientId.IsMissing())
            {
                return ClientError.MissingClientId;
            }
            
            if (clientId.Length > 50)
            {
                return ClientError.InvalidClientId;
            }
            
            if (clientSecret.IsMissing())
            {
                return ClientError.MissingClientSecret;
            }
            
            if (clientSecret.Length > 250)
            {
                return ClientError.InvalidClientSecret;
            }
            
            Apply(new UpdateClientEvent(name, clientId, clientId, isEnable));
            return Result.Ok();
        }

        public async Task<Result> AddPermissionAsync(Common.Permission permission)
        {
            if (permission == null)
            {
                return ClientError.InvalidPermission;
            }

            if (!await _permissions.ExistAsync(permission.Id)
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

        public async Task<Result> AddRoleAsync(Common.Role role)
        {
            if (role == null)
            {
                return ClientError.InvalidRole;
            }
            
            if (!await _roles.ExistAsync(role.Id)
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

        public Result RemoveRoleAsync(Common.Role role)
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

        public async Task<Result> AddResourceAsync(Common.Resource resource)
        {
            if (resource == null)
            {
                return ClientError.InvalidRole;
            }
            
            if (!await _resources.ExistAsync(resource.Id)
                .ConfigureAwait(false))
            {
                return ClientError.InvalidRole;
            }
            
            if (State.Resources.Contains(resource))
            {
                return ClientError.RoleAlreadyExist;
            }
            
            Apply(new AddResourceEvent(resource));
            return Result.Ok();
        }

        public async Task<Result> RemoveResourceAsync(Common.Resource resource)
        {
            if (resource == null)
            {
                return ClientError.InvalidRole;
            }
            
            if (!await _resources.ExistAsync(resource.Id)
                .ConfigureAwait(false))
            {
                return ClientError.InvalidRole;
            }
            
            if (!State.Resources.Contains(resource))
            {
                return ClientError.NotContainsRole;
            }
            
            Apply(new RemoveResourceEvent(resource));
            return Result.Ok();
        }
    }
}