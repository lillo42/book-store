using System;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Client.Events
{
    public class RemovePermissionEvent : Event
    {
        public RemovePermissionEvent(Common.Permission permission)
        {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        }

        public Common.Permission Permission { get; }
    }
}