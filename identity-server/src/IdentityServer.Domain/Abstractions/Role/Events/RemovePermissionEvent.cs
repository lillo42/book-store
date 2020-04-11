using System;

namespace IdentityServer.Domain.Abstractions.Role.Events
{
    public class RemovePermissionEvent : IEvent
    {
        public RemovePermissionEvent(Common.Permission permission)
        {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        }

        public Common.Permission Permission { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        
        string IEvent.Name => nameof(RemovePermissionEvent);
    }
}