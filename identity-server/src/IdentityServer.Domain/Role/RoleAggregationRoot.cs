using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Role;
using IdentityServer.Domain.Abstractions.Role.Events;
using IdentityServer.Domain.Extensions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;
using static IdentityServer.Domain.DomainError;

namespace IdentityServer.Domain.Role
{
    public class RoleAggregationRoot : AggregateRoot<RoleState, Guid>, IRoleAggregationRoot
    {
        private readonly IReadOnlyRoleRepository _roleRepository;
        private readonly IReadOnlyPermissionRepository _permissionsRepository;
        
        public RoleAggregationRoot(RoleState state,
            IReadOnlyRoleRepository roleRepository,
            IReadOnlyPermissionRepository permissionsRepository,
            ILogger<RoleAggregationRoot> logger) 
            : base(state, logger)
        {
            _permissionsRepository = permissionsRepository ?? throw new ArgumentNullException(nameof(permissionsRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        public async Task<Result> CreateAsync(string name, string displayName, string description, CancellationToken cancellationToken = default)
        {
            if (name.IsMissing())
            {
                return RoleError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return RoleError.InvalidName;
            }

            if (await _roleRepository.ExistAsync(name, cancellationToken)
                .ConfigureAwait(false))
            {
                return RoleError.NameAlreadyExist;
            }
            
            if (displayName.IsMissing())
            {
                return RoleError.MissingDisplayName;
            }

            if (displayName.Length > 50)
            {
                return RoleError.InvalidDisplayName;
            }
            
            if (description != null && description.Length > 250)
            {
                return RoleError.InvalidDescription;
            }
            
            Apply(new CreateRoleEvent(name, displayName, description));
            return Result.Ok();
        }
        
        public async Task<Result> UpdateAsync(string name, string displayName, string description, CancellationToken cancellationToken = default)
        {
            if (name.IsMissing())
            {
                return RoleError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return RoleError.InvalidName;
            }

            if (State.Name != name && await _roleRepository.ExistAsync(name).ConfigureAwait(false))
            {
                return RoleError.NameAlreadyExist;
            }
            
            if (displayName.IsMissing())
            {
                return RoleError.MissingDisplayName;
            }
            
            if (displayName.Length > 50)
            {
                return RoleError.InvalidDisplayName;
            }
            
            if (description != null && description.Length > 250)
            {
                return RoleError.InvalidDescription;
            }
            
            Apply(new UpdateRoleEvent(name, displayName, description));
            return Result.Ok();
        }

        public async Task<Result> AddPermission(Common.Permission permission)
        {
            if (permission == null)
            {
                return RoleError.InvalidPermission;
            }
            
            if (!await _permissionsRepository.ExistAsync(permission.Id)
                .ConfigureAwait(false))
            {
                return RoleError.InvalidPermission;
            }

            if (State.Permissions.Contains(permission))
            {
                return RoleError.PermissionAlreadyExist;
            }
            
            Apply(new AddPermissionEvent(permission));
            return Result.Ok();
        }

        public Result RemovePermission(Common.Permission permission)
        {
            if (permission == null)
            {
                return RoleError.InvalidPermission;
            }

            if (!State.Permissions.Contains(permission))
            {
                return RoleError.NotContainsPermission;
            }
            
            Apply(new RemovePermissionEvent(permission));
            return Result.Ok();
        }
    }
}