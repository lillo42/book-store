using System;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Role.Events
{
    public class CreateRoleEvent : Event
    {
        public CreateRoleEvent(string name, string displayName, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Description = description;
        }

        public string Name { get; }
        
        public string DisplayName { get; }
        public string Description { get; }
    }
}