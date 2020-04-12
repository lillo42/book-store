using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.User;
using IdentityServer.Domain.Abstractions.User.Events;
using IdentityServer.Domain.Extensions;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

using static IdentityServer.Domain.DomainError;

namespace IdentityServer.Domain.User
{
    public class UserAggregationRoot : AggregateRoot<UserState,Guid>, IUserAggregationRoot
    {
        private readonly IHashAlgorithm _hash;
        private readonly IReadOnlyPermissionRepository _permissions;
        private readonly IReadOnlyRoleRepository _role;
        
        public UserAggregationRoot(UserState state, 
            ILogger<UserAggregationRoot> logger, 
            IHashAlgorithm hash, 
            IReadOnlyPermissionRepository permissions, 
            IReadOnlyRoleRepository role) 
            : base(state, logger)
        {
            _hash = hash ?? throw new ArgumentNullException(nameof(hash));
            _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            _role = role ?? throw new ArgumentNullException(nameof(role));
        }
        
        public Result Create(string mail, string password, bool isEnable)
        {
            if (mail.IsMissing())
            {
                return UserError.MissingMail;
            }
            
            if(!Regex.IsMatch(mail, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
            {
                return UserError.InvalidMail;
            }
            
            if (mail.Length > 100)
            {
                return UserError.InvalidMail;
            }

            if (password.IsMissing())
            {
                return UserError.MissingPassword;
            }
            
            if (password.Length < 3)
            {
                return UserError.InvalidPassword;
            }
            
            Apply(new CreateUserEvent(mail, _hash.ComputeHash(password), isEnable));
            return Result.Ok();
        }

        public Result Update(string mail, bool isEnable)
        {
            if (mail.IsMissing())
            {
                return UserError.MissingMail;
            }
            
            if(!Regex.IsMatch(mail, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
            {
                return UserError.InvalidMail;
            }
            
            if (mail.Length > 100)
            {
                return UserError.InvalidMail;
            }

            Apply(new UpdateUserEvent(mail, isEnable));
            return Result.Ok();
        }

        public async Task<Result> AddPermissionAsync(Common.Permission permission)
        {
            if (permission == null)
            {
                return UserError.InvalidPermission;
            }

            if (!await _permissions.ExistAsync(permission.Id)
                .ConfigureAwait(false))
            {
                return UserError.InvalidPermission;
            }
            
            if (State.Permissions.Contains(permission))
            {
                return UserError.PermissionAlreadyExist;
            }
            
            Apply(new AddPermissionEvent(permission));
            return Result.Ok();
        }

        public async Task<Result> RemovePermissionAsync(Common.Permission permission)
        {
            if (permission == null)
            {
                return UserError.InvalidPermission;
            }

            if (!await _permissions.ExistAsync(permission.Id)
                .ConfigureAwait(false))
            {
                return UserError.InvalidPermission;
            }
            
            if (!State.Permissions.Contains(permission))
            {
                return UserError.NotContainsPermission;
            }
            
            Apply(new RemovePermissionEvent(permission));
            return Result.Ok();
        }

        public async Task<Result> AddRoleAsync(Common.Role role)
        {
            if (role == null)
            {
                return UserError.InvalidRole;
            }
            
            if (!await _role.ExistAsync(role.Id)
                .ConfigureAwait(false))
            {
                return UserError.InvalidRole;
            }
            
            if (State.Roles.Contains(role))
            {
                return UserError.RoleAlreadyExist;
            }
            
            Apply(new AddRoleEvent(role));
            return Result.Ok();
        }

        public async Task<Result> RemoveRoleAsync(Common.Role role)
        {
            if (role == null)
            {
                return UserError.InvalidRole;
            }
            
            if (!await _role.ExistAsync(role.Id)
                .ConfigureAwait(false))
            {
                return UserError.InvalidRole;
            }
            
            if (!State.Roles.Contains(role))
            {
                return UserError.NotContainsRole;
            }
            
            Apply(new RemoveRoleEvent(role));
            return Result.Ok();
        }
    }
}