using System;

namespace IdentityServer.Domain.Common
{
    public abstract class Event
    {
        public string Id { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}