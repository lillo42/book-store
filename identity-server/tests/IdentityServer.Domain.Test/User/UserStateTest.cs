using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain.Abstractions.User;
using IdentityServer.Domain.Abstractions.User.Events;
using Xunit;

namespace IdentityServer.Domain.Test.User
{
    public class UserStateTest
    {
        private readonly UserState _state;
        private readonly Common.User _entity;
        private readonly Fixture _fixture;

        public UserStateTest()
        {
            _fixture = new Fixture();

            _entity = _fixture.Create<Common.User>();
            _state = new UserState(_entity);
        }

        [Fact]
        public void Apply_CreateUser()
        {
            var mail = _fixture.Create<string>();
            var password = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            var @event = new CreateUserEvent(mail, password, isEnable);
            _state.Apply(@event);
            
            _entity.Mail.Should().Be(mail);
            _entity.Password.Should().Be(password);
            _entity.IsEnable.Should().Be(isEnable);
        }
        
        [Fact]
        public void Apply_UpdateRole()
        {
            var mail = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            var @event = new UpdateUserEvent(mail, isEnable);
            _state.Apply(@event);
            
            _entity.Mail.Should().Be(mail);
            _entity.IsEnable.Should().Be(isEnable);
        }
        
        [Fact]
        public void Apply_AddPermission()
        {
            var counter = _entity.Permissions.Count;
            var permission = _fixture.Create<Common.Permission>();
            
            var @event = new AddPermissionEvent(permission);
            _state.Apply(@event);

            _entity.Permissions.Should().NotBeNull();
            _entity.Permissions.Should().NotBeEmpty();
            _entity.Permissions.Should().HaveCount(counter + 1);
            _entity.Permissions.Should().Contain(permission);
        }
        
        [Fact]
        public void Apply_RemovePermission()
        {
            var permission = _fixture.Create<Common.Permission>();
            _entity.Permissions.Add(permission);
            var count = _entity.Permissions.Count;
            
            var @event = new RemovePermissionEvent(permission);
            _state.Apply(@event);

            _entity.Permissions.Should().HaveCount(count - 1);
            _entity.Permissions.Should().NotContain(permission);
        }
        
        [Fact]
        public void Apply_AddRole()
        {
            var counter = _entity.Roles.Count;
            var permission = _fixture.Create<Common.Role>();
            
            var @event = new AddRoleEvent(permission);
            _state.Apply(@event);

            _entity.Roles.Should().NotBeNull();
            _entity.Roles.Should().NotBeEmpty();
            _entity.Roles.Should().HaveCount(counter + 1);
            _entity.Roles.Should().Contain(permission);
        }
        
        [Fact]
        public void Apply_RemoveRole()
        {
            var role = _fixture.Create<Common.Role>();
            _entity.Roles.Add(role);
            var count = _entity.Roles.Count;
            
            var @event = new RemoveRoleEvent(role);
            _state.Apply(@event);

            _entity.Roles.Should().HaveCount(count - 1);
            _entity.Roles.Should().NotContain(role);
        }
    }
}