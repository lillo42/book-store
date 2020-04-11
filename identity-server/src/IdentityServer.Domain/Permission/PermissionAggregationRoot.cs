using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Permission;
using IdentityServer.Domain.Abstractions.Permission.Events;
using IdentityServer.Domain.Abstractions.Role.Events;
using IdentityServer.Domain.Extensions;
using Microsoft.Extensions.Logging;

using static IdentityServer.Domain.DomainError;

namespace IdentityServer.Domain.Permission
{
    public class PermissionAggregationRoot : AggregateRoot<PermissionState, Guid>, IPermissionAggregationRoot
    {
        public PermissionAggregationRoot(PermissionState state, 
            ILogger<PermissionAggregationRoot> logger) 
            : base(state, logger)
        {
        }
        
        public Result Create(string name, string displayName, string description)
        {
            if (name.IsMissing())
            {
                return PermissionError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return PermissionError.InvalidName;
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
        
        public Result Update(string name, string displayName, string description)
        {
            if (name.IsMissing())
            {
                return PermissionError.MissingName;
            }
            
            if (name.Length > 20)
            {
                return PermissionError.InvalidName;
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