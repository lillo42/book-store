using IdentityServer.Domain.Common;

namespace IdentityServer.Domain.Abstractions.Client.Events
{
    public class AddResourceEvent : Event
    {
        public AddResourceEvent(Common.Resource resource)
        {
            Resource = resource;
        }

        public Common.Resource Resource { get; }
    }
}