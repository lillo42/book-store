using System;
using IdentityServer.Domain.Abstractions.User.Events;
using IdentityServer.Infrastructure;

namespace IdentityServer.Domain.Abstractions.User
{
    public class UserState : IState<Guid>
    {
        private readonly Common.User _user;

        public UserState(Common.User user)
        {
            _user = user;
            Roles = new HashSetTrace<Common.Role>(user.Roles);
            Permissions = new HashSetTrace<Common.Permission>(user.Permissions);
        }

        public Guid Id => _user.Id;

        public string Mail => _user.Mail;
        public string Password => _user.Password;
        public bool IsEnable => _user.IsEnable;

        public HashSetTrace<Common.Role> Roles { get; }
        public HashSetTrace<Common.Permission> Permissions { get; }

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
            Permissions.Add(@event.Permission);
        }

        public void Apply(RemovePermissionEvent @event)
        {
            Permissions.Remove(@event.Permission);
        }
        
        public void Apply(AddRoleEvent @event)
        {
            Roles.Add(@event.Role);
        }

        public void Apply(RemoveRoleEvent @event)
        {
            Roles.Remove(@event.Role);
        }
        
        public static explicit operator Common.User(UserState state)
            => state._user;
    }
}