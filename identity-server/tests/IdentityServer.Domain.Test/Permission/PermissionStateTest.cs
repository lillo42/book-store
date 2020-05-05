using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain.Abstractions.Permission;
using IdentityServer.Domain.Abstractions.Permission.Events;
using Xunit;

namespace IdentityServer.Domain.Test.Permission
{
    public class PermissionStateTest
    {
        private readonly PermissionState _state;
        private readonly Common.Permission _entity;
        private readonly Fixture _fixture;

        public PermissionStateTest()
        {
            _fixture = new Fixture();

            _entity = _fixture.Create<Common.Permission>();
            _state = new PermissionState(_entity);
        }

        [Fact]
        public void Apply_CreatePermission()
        {
            var name = _fixture.Create<string>();
            var displayName = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            
            var @event = new CreatePermissionEvent(name, displayName, description);
            _state.Apply(@event);

            _entity.Name.Should().Be(name);
            _entity.DisplayName.Should().Be(displayName);
            _entity.Description.Should().Be(description);
        }
        
        [Fact]
        public void Apply_UpdatePermission()
        {
            var name = _fixture.Create<string>();
            var displayName = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            
            var @event = new UpdatePermissionEvent(name, displayName, description);
            _state.Apply(@event);

            _entity.Name.Should().Be(name);
            _entity.DisplayName.Should().Be(displayName);
            _entity.Description.Should().Be(description);
        }
    }
}