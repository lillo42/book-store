using System;

namespace IdentityServer.Domain.Abstractions.Role.Events
{
    public class AddPermissionEvent : IEvent
    {
        public AddPermissionEvent(Common.Permission permissionId)
        {
            Permission = permissionId ?? throw new ArgumentNullException(nameof(permissionId));
        }

        public Common.Permission Permission { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        string IEvent.Name => nameof(AddPermissionEvent);
    }
}