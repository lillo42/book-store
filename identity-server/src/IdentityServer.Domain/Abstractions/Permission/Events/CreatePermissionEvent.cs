using System;

namespace IdentityServer.Domain.Abstractions.Permission.Events
{
    public class CreatePermissionEvent : IEvent
    {
        public CreatePermissionEvent(string name, string displayName, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Description = description;
        }

        public string Name { get; }
        
        public string DisplayName { get; }
        public string Description { get; }
        
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        
        string IEvent.Name => nameof(CreatePermissionEvent);
    }
}