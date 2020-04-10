using System;

namespace IdentityServer.Domain.Abstractions
{
    public interface IEvent
    {
        string Name { get; }
        DateTime OccurredOn { get; }
    }
}