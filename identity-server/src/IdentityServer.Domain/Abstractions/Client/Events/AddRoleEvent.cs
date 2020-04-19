using System;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Client.Events
{
    public class AddRoleEvent : Event
    {
        public AddRoleEvent(Common.Role permissionId)
        {
            Role = permissionId ?? throw new ArgumentNullException(nameof(permissionId));
        }

        public Common.Role Role { get; }
    }
}