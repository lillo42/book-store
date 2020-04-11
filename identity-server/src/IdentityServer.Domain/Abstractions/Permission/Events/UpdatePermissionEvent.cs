using System;

namespace IdentityServer.Domain.Abstractions.Permission.Events
{
    public class UpdatePermissionEvent : IEvent
    {
        public UpdatePermissionEvent(string name, string displayName, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
        public string DisplayName { get; }
        
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        
        string IEvent.Name => nameof(UpdatePermissionEvent);
    }
}