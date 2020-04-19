using System;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Client.Events
{
    public class RemoveRoleEvent : Event
    {
        public RemoveRoleEvent(Common.Role role)
        {
            Role = role ?? throw new ArgumentNullException(nameof(role));
        }

        public Common.Role Role { get; }
    }
}