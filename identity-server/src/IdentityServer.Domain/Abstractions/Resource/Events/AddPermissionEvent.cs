using System;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Resource.Events
{
    public class AddPermissionEvent : Event
    {
        public AddPermissionEvent(Common.Permission permissionId)
        {
            Permission = permissionId ?? throw new ArgumentNullException(nameof(permissionId));
        }

        public Common.Permission Permission { get; }
    }
}