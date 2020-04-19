using System;

namespace IdentityServer.Domain.Abstractions.Client
{
    public interface IClientAggregationStore : IAggregateStore<IClientAggregationRoot, ClientState, Guid>
    {
        
    }
}