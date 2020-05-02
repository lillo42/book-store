using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain.Abstractions.Resource;
using IdentityServer.Domain.Abstractions.Resource.Events;
using Xunit;

namespace IdentityServer.Domain.Test.Resource
{
    public class ResourceStateTest
    {
        private readonly ResourceState _state;
        private readonly Common.Resource _entity;
        private readonly Fixture _fixture;

        public ResourceStateTest()
        {
            _fixture = new Fixture();

            _entity = _fixture.Create<Common.Resource>();
            _state = new ResourceState(_entity);
        }

        [Fact]
        public void Apply_CreateResource()
        {
            var name = _fixture.Create<string>();
            var displayName = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            var @event = new CreateResourceEvent(name, displayName, description, isEnable);
            _state.Apply(@event);

            _entity.Name.Should().Be(name);
            _entity.DisplayName.Should().Be(displayName);
            _entity.Description.Should().Be(description);
            _entity.IsEnable.Should().Be(isEnable);
        }
        
        [Fact]
        public void Apply_UpdateResource()
        {
            var name = _fixture.Create<string>();
            var displayName = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            var @event = new UpdateResourceEvent(name, displayName, description, isEnable);
            _state.Apply(@event);

            _entity.Name.Should().Be(name);
            _entity.DisplayName.Should().Be(displayName);
            _entity.Description.Should().Be(description);
            _entity.IsEnable.Should().Be(isEnable);
        }
    }
}