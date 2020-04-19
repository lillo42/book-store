using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Client.Events
{
    public class RemoveResourceEvent : Event
    {
        public RemoveResourceEvent(Common.Resource resource)
        {
            Resource = resource;
        }

        public Common.Resource Resource { get; }
    }
}