using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Role;
using IdentityServer.Domain.Common;
using IdentityServer.Domain.Extensions;
using IdentityServer.Domain.Roles.Events;
using Microsoft.Extensions.Logging;
using static IdentityServer.Domain.DomainError;

namespace IdentityServer.Domain.Roles
{
    public class RoleAggregationRoot : AggregateRoot<RoleState, Guid>, IRoleAggregationRoot
    {
        public RoleAggregationRoot(RoleState state, 
            ILogger<RoleAggregationRoot> logger) 
            : base(state, logger)
        {
        }

        public Result Create(string name, string displayName, string description)
        {
            if (name.IsMissing())
            {
                return RoleError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return RoleError.InvalidName;
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
        
        public Result Update(string name, string displayName, string description)
        {
            if (name.IsMissing())
            {
                return RoleError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return RoleError.InvalidName;
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

        public Result AddPermission(Permission permission)
        {
            if (permission == null)
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

        public Result RemovePermission(Permission permission)
        {
            if (permission == null)
            {
                return RoleError.InvalidPermission;
            }
            
            if (!State.Permissions.Contains(permission))
            {
                return RoleError.PermissionAlreadyExist;
            }
            
            Apply(new RemovePermissionEvent(permission));
            return Result.Ok();
        }
    }
}