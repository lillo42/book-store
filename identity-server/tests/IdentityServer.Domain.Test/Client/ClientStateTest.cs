using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain.Abstractions.Client;
using IdentityServer.Domain.Abstractions.Client.Events;
using IdentityServer.Infrastructure;
using Xunit;

namespace IdentityServer.Domain.Test.Client
{
    public class ClientStateTest
    {
        private readonly ClientState _state;
        private readonly Common.Client _entity;
        private readonly Fixture _fixture;

        public ClientStateTest()
        {
            _fixture = new Fixture();

            _entity = _fixture.Create<Common.Client>();
            _state = new ClientState(_entity);
        }

        [Fact]
        public void Apply_CreateUser()
        {
            var name = _fixture.Create<string>();
            var clientId = _fixture.Create<string>();
            var clientSecret = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();

            var @event = new CreateClientEvent(name, clientId, clientSecret, isEnable);
            _state.Apply(@event);
            
            _entity.Name.Should().Be(name);
            _entity.ClientId.Should().Be(clientId);
            _entity.ClientSecret.Should().Be(clientSecret);
            _entity.IsEnable.Should().Be(isEnable);
        }
        
        [Fact]
        public void Apply_UpdateRole()
        {
            var name = _fixture.Create<string>();
            var clientId = _fixture.Create<string>();
            var clientSecret = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();

            var @event = new CreateClientEvent(name, clientId, clientSecret, isEnable);
            _state.Apply(@event);
            
            _entity.Name.Should().Be(name);
            _entity.ClientId.Should().Be(clientId);
            _entity.ClientSecret.Should().Be(clientSecret);
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
            
            _state.Permissions.Traces.Should().HaveCount(counter + 1);
            _state.Permissions.Traces.Should().Contain(x => x.Value.Equals(permission) && x.State == State.Added);
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
            
            _state.Permissions.Traces.Should().HaveCount(count);
            _state.Permissions.Traces.Should().Contain(x => x.Value.Equals(permission) && x.State == State.Removed);
        }
        
        [Fact]
        public void Apply_AddRole()
        {
            var counter = _entity.Roles.Count;
            var role = _fixture.Create<Common.Role>();
            
            var @event = new AddRoleEvent(role);
            _state.Apply(@event);

            _entity.Roles.Should().NotBeNull();
            _entity.Roles.Should().NotBeEmpty();
            _entity.Roles.Should().HaveCount(counter + 1);
            _entity.Roles.Should().Contain(role);
            
            _state.Roles.Traces.Should().HaveCount(counter + 1);
            _state.Roles.Traces.Should().Contain(x => x.Value.Equals(role) && x.State == State.Added);
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
            
            _state.Roles.Traces.Should().HaveCount(count);
            _state.Roles.Traces.Should().Contain(x => x.Value.Equals(role) && x.State == State.Removed);
        }
        
        [Fact]
        public void Apply_AddResource()
        {
            var counter = _entity.Resources.Count;
            var resource = _fixture.Create<Common.Resource>();
            
            var @event = new AddResourceEvent(resource);
            _state.Apply(@event);

            _entity.Resources.Should().NotBeNull();
            _entity.Resources.Should().NotBeEmpty();
            _entity.Resources.Should().HaveCount(counter + 1);
            _entity.Resources.Should().Contain(resource);
            
            _state.Resources.Traces.Should().HaveCount(counter + 1);
            _state.Resources.Traces.Should().Contain(x => x.Value.Equals(resource) && x.State == State.Added);
        }
        
        [Fact]
        public void Apply_RemoveResource()
        {
            var resource = _fixture.Create<Common.Resource>();
            _entity.Resources.Add(resource);
            var count = _entity.Roles.Count;
            
            var @event = new RemoveResourceEvent(resource);
            _state.Apply(@event);

            _entity.Resources.Should().HaveCount(count - 1);
            _entity.Resources.Should().NotContain(resource);

            _state.Resources.Traces.Should().HaveCount(count);
            _state.Resources.Traces.Should().Contain(x => x.Value.Equals(resource) && x.State == State.Removed);
        }
    }
}