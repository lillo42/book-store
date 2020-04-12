using System;

namespace IdentityServer.Domain.Common
{
    public abstract class Event
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}