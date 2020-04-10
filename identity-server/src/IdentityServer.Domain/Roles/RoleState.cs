using System;
using System.Collections.Generic;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Common;
using IdentityServer.Domain.Roles.Events;

namespace IdentityServer.Domain.Roles
{
    public class RoleState : IState<Guid>
    {
        private readonly Common.Role _role;

        public RoleState(Common.Role role)
        {
            _role = role ?? throw new ArgumentNullException(nameof(role));
        }

        public Guid Id => _role.Id;
        
        public string Name => _role.Name;
        
        public string Description => _role.Description;
        
        public string DisplayName => _role.DisplayName;
        
        public ISet<Permission> Permissions => _role.Permissions;
        
        
        public void Apply(CreateRoleEvent @event)
        {
            _role.Name = @event.Name;
            _role.Description = @event.Description;
            _role.DisplayName = @event.DisplayName;
        }
        
        public void Apply(UpdateRoleEvent @event)
        {
            _role.Name = @event.Name;
            _role.Description = @event.Description;
            _role.DisplayName = @event.DisplayName;
        }

        public void Apply(AddPermissionEvent @event)
        {
            _role.Permissions.Add(@event.Permission);
        }

        public void Apply(RemovePermissionEvent @event)
        {
            _role.Permissions.Remove(@event.Permission);
        }
        
        public static explicit operator Common.Role(RoleState state)
            => state._role;
    }
}