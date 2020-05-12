using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.User;
using IdentityServer.Domain.Abstractions.User.Events;
using IdentityServer.Domain.Extensions;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

using static IdentityServer.Domain.DomainError;

namespace IdentityServer.Domain.User
{
    public class UserAggregationRoot : AggregateRoot<UserState,Guid>, IUserAggregationRoot
    {
        private readonly IHashAlgorithm _hash;
        private readonly IReadOnlyUserRepository _userRepository;
        private readonly IReadOnlyPermissionRepository _permissionRepository;
        private readonly IReadOnlyRoleRepository _roleRepository;
        
        public UserAggregationRoot(
            UserState state, 
            IHashAlgorithm hash, 
            IReadOnlyUserRepository userRepository,
            IReadOnlyPermissionRepository permissionsRepository, 
            IReadOnlyRoleRepository roleRepository,
            ILogger<UserAggregationRoot> logger) 
            : base(state, logger)
        {
            _hash = hash ?? throw new ArgumentNullException(nameof(hash));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _permissionRepository = permissionsRepository ?? throw new ArgumentNullException(nameof(permissionsRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }
        
        public async Task<Result> CreateAsync(string mail, string password, bool isEnable, CancellationToken cancellationToken = default)
        {
            if (mail.IsMissing())
            {
                return UserError.MissingMail;
            }
            
            if (mail.Length > 100)
            {
                return UserError.InvalidMail;
            }
            
            if(!Regex.IsMatch(mail, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
            {
                return UserError.InvalidMail;
            }

            if (await _userRepository.ExistAsync(mail, cancellationToken).ConfigureAwait(false))
            {
                return UserError.MailAlreadyExist;
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

        public async Task<Result> UpdateAsync(string mail, bool isEnable, CancellationToken cancellationToken = default)
        {
            if (mail.IsMissing())
            {
                return UserError.MissingMail;
            }
            
            if (mail.Length > 100)
            {
                return UserError.InvalidMail;
            }
            
            if(!Regex.IsMatch(mail, @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$"))
            {
                return UserError.InvalidMail;
            }
            
            if (State.Mail != mail && await _userRepository.ExistAsync(mail, cancellationToken).ConfigureAwait(false))
            {
                return UserError.MailAlreadyExist;
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

            if (!await _permissionRepository.ExistAsync(permission.Id)
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

        public Result RemovePermission(Common.Permission permission)
        {
            if (permission == null)
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
            
            if (!await _roleRepository.ExistAsync(role.Id)
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

        public Result RemoveRole(Common.Role role)
        {
            if (role == null)
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