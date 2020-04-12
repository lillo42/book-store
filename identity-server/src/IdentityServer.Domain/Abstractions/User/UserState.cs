using System;
using System.Collections.Generic;
using IdentityServer.Domain.Abstractions.User.Events;

namespace IdentityServer.Domain.Abstractions.User
{
    public class UserState : IState<Guid>
    {
        private readonly Common.User _user;

        public UserState(Common.User user)
        {
            _user = user;
        }

        public Guid Id => _user.Id;

        public string Mail => _user.Mail;
        public string Password => _user.Password;
        public bool IsEnable => _user.IsEnable;

        public ISet<Common.Role> Roles => _user.Roles;
        public ISet<Common.Permission> Permissions => _user.Permissions;
        
        public bool RolesHasChange { get; private set; }
        public bool PermissionsHasChange { get; private set; }

        public void Apply(CreateUserEvent @event)
        {
            _user.Mail = @event.Mail;
            _user.Password = @event.Password;
            _user.IsEnable = @event.IsEnable;
        }

        public void Apply(UpdateUserEvent @event)
        {
            _user.Mail = @event.Mail;
            _user.IsEnable = @event.IsEnable;
        }

        public void Apply(AddPermissionEvent @event)
        {
            _user.Permissions.Add(@event.Permission);
            PermissionsHasChange = true;
        }

        public void Apply(RemovePermissionEvent @event)
        {
            _user.Permissions.Remove(@event.Permission);
            PermissionsHasChange = true;
        }
        
        public void Apply(AddRoleEvent @event)
        {
            _user.Roles.Add(@event.Role);
            RolesHasChange = true;
        }

        public void Apply(RemoveRoleEvent @event)
        {
            _user.Roles.Remove(@event.Role);
            RolesHasChange = true;
        }
        
        public static explicit operator Common.User(UserState state)
            => state._user;
    }
}