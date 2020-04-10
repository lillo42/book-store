using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Roles.Events
{
    public class RemovePermissionEvent : IEvent
    {
        public RemovePermissionEvent(Permission permission)
        {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        }

        public Permission Permission { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        
        string IEvent.Name => nameof(RemovePermissionEvent);
    }
}