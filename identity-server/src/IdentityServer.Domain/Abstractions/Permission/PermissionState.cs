using System;
using IdentityServer.Domain.Abstractions.Permission.Events;

namespace IdentityServer.Domain.Abstractions.Permission
{
    public class PermissionState : IState<Guid>
    {
        private readonly Common.Permission _permission;

        public PermissionState(Common.Permission permission)
        {
            _permission = permission ?? throw new ArgumentNullException(nameof(permission));
        }

        public Guid Id => _permission.Id;
        
        public string Name => _permission.Name;
        
        public string Description => _permission.Description;
        
        public string DisplayName => _permission.DisplayName;
        
        public void Apply(CreatePermissionEvent @event)
        {
            _permission.Name = @event.Name;
            _permission.Description = @event.Description;
            _permission.DisplayName = @event.DisplayName;
        }
        
        public void Apply(UpdatePermissionEvent @event)
        {
            _permission.Name = @event.Name;
            _permission.Description = @event.Description;
            _permission.DisplayName = @event.DisplayName;
        }
        
        public static explicit operator Common.Permission(PermissionState state)
            => state._permission;
    }
}