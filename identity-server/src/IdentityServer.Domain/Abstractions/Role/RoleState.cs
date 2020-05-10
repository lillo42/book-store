using System;
using IdentityServer.Domain.Abstractions.Role.Events;
using IdentityServer.Infrastructure;

namespace IdentityServer.Domain.Abstractions.Role
{
    public class RoleState : IState<Guid>
    {
        private readonly Common.Role _role;

        public RoleState(Common.Role role)
        {
            _role = role ?? throw new ArgumentNullException(nameof(role));
            Permissions = new HashSetTrace<Common.Permission>(_role.Permissions);
        }

        public Guid Id => _role.Id;
        
        public string Name => _role.Name;
        
        public string Description => _role.Description;
        
        public string DisplayName => _role.DisplayName;
        
        public HashSetTrace<Common.Permission> Permissions { get; }
        
        
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
            Permissions.Add(@event.Permission);
        }

        public void Apply(RemovePermissionEvent @event)
        {
            Permissions.Remove(@event.Permission);
        }
        
        public static explicit operator Common.Role(RoleState state)
            => state._role;
    }
}