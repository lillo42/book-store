using System;
using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Resource.Events
{
    public class CreateResourceEvent : Event
    {
        public CreateResourceEvent(string name, string displayName, string description, bool isEnable)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Description = description;
            IsEnable = isEnable;
        }

        public string Name { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public bool IsEnable { get; }
    }
}