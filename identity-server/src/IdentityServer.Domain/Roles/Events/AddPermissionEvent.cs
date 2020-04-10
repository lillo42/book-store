using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Roles.Events
{
    public class AddPermissionEvent : IEvent
    {
        public AddPermissionEvent(Permission permissionId)
        {
            Permission = permissionId ?? throw new ArgumentNullException(nameof(permissionId));
        }

        public Permission Permission { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        string IEvent.Name => nameof(AddPermissionEvent);
    }
}