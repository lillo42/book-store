using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Permission;
using IdentityServer.Domain.Abstractions.Permission.Events;
using IdentityServer.Domain.Extensions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

using static IdentityServer.Domain.DomainError;

namespace IdentityServer.Domain.Permission
{
    public class PermissionAggregationRoot : AggregateRoot<PermissionState, Guid>, IPermissionAggregationRoot
    {
        private readonly IReadOnlyPermissionRepository _repository;
        public PermissionAggregationRoot(PermissionState state,
            IReadOnlyPermissionRepository repository,
            ILogger<PermissionAggregationRoot> logger) 
            : base(state, logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public async Task<Result> CreateAsync(string name, string displayName, string description, CancellationToken cancellationToken = default)
        {
            if (name.IsMissing())
            {
                return PermissionError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return PermissionError.InvalidName;
            }
            
            if (await _repository.ExistAsync(name, cancellationToken)
                .ConfigureAwait(false))
            {
                return PermissionError.NameAlreadyExist;
            }

            if (displayName.IsMissing())
            {
                return PermissionError.MissingDisplayName;
            }
            
            if (displayName.Length > 50)
            {
                return PermissionError.InvalidDisplayName;
            }
            
            if (description != null && description.Length > 250)
            {
                return PermissionError.InvalidDescription;
            }
            
            Apply(new CreatePermissionEvent(name, displayName, description));
            return Result.Ok();
        }
        
        public async Task<Result> UpdateAsync(string name, string displayName, string description,
            CancellationToken cancellationToken = default)
        {
            if (name.IsMissing())
            {
                return PermissionError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return PermissionError.InvalidName;
            }
            
            if (State.Name != name && await _repository.ExistAsync(name, cancellationToken)
                .ConfigureAwait(false))
            {
                return PermissionError.NameAlreadyExist;
            }
            
            if (displayName.IsMissing())
            {
                return PermissionError.MissingDisplayName;
            }
            
            if (displayName.Length > 50)
            {
                return PermissionError.InvalidDisplayName;
            }
            
            if (description != null && description.Length > 250)
            {
                return PermissionError.InvalidDescription;
            }
            
            Apply(new UpdatePermissionEvent(name, displayName, description));
            return Result.Ok();
        }
    }
}