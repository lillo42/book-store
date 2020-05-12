using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain.Abstractions.Role;
using IdentityServer.Domain.Abstractions.Role.Events;
using Xunit;

namespace IdentityServer.Domain.Test.Role
{
    public class RoleStateTest
    {
        private readonly RoleState _state;
        private readonly Common.Role _entity;
        private readonly Fixture _fixture;

        public RoleStateTest()
        {
            _fixture = new Fixture();

            _entity = _fixture.Create<Common.Role>();
            _state = new RoleState(_entity);
        }

        [Fact]
        public void Apply_CreateRole()
        {
            var name = _fixture.Create<string>();
            var displayName = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            
            var @event = new CreateRoleEvent(name, displayName, description);
            _state.Apply(@event);

            _entity.Name.Should().Be(name);
            _entity.DisplayName.Should().Be(displayName);
            _entity.Description.Should().Be(description);
        }
        
        [Fact]
        public void Apply_UpdateRole()
        {
            var name = _fixture.Create<string>();
            var displayName = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            
            var @event = new UpdateRoleEvent(name, displayName, description);
            _state.Apply(@event);

            _entity.Name.Should().Be(name);
            _entity.DisplayName.Should().Be(displayName);
            _entity.Description.Should().Be(description);
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
    }
}