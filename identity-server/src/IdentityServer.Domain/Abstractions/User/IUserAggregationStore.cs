using System;

namespace IdentityServer.Domain.Abstractions.User
{
    public interface IUserAggregationStore : IAggregateStore<IUserAggregationRoot, UserState, Guid>
    {
        
    }
}