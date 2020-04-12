using System;
using IdentityServer.Domain.Abstractions.Resource.Events;

namespace IdentityServer.Domain.Abstractions.Resource
{
    public class ResourceState : IState<Guid>
    {
        private readonly Common.Resource _resource;

        public ResourceState(Common.Resource role)
        {
            _resource = role ?? throw new ArgumentNullException(nameof(role));
        }

        public Guid Id => _resource.Id;
        
        public string Name => _resource.Name;
        
        public string Description => _resource.Description;
        
        public string DisplayName => _resource.DisplayName;
        
        public bool IsEnable => _resource.IsEnable;
        
        
        public void Apply(CreateResourceEvent @event)
        {
            _resource.Name = @event.Name;
            _resource.Description = @event.Description;
            _resource.DisplayName = @event.DisplayName;
            _resource.IsEnable = @event.IsEnable;
        }
        
        public void Apply(UpdateResourceEvent @event)
        {
            _resource.Name = @event.Name;
            _resource.Description = @event.Description;
            _resource.DisplayName = @event.DisplayName;
            _resource.IsEnable = @event.IsEnable;
        }

        public static explicit operator Common.Resource(ResourceState state)
            => state._resource;
    }
}