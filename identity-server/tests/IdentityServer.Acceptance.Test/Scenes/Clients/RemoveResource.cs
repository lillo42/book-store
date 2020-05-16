using System;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Acceptance.Test.Extensions;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using TestStack.BDDfy;
using Xunit;

namespace IdentityServer.Acceptance.Test.Scenes.Clients
{
    public class RemoveResource : BaseScene
    {
        private Client _client;
        private Resource _resource;
        private RemoveClientResourceReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemoveResource_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemoveResource(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemoveResource_Should_ReturnInvalid_When_ResourceIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemoveResource(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void RemoveResource_Should_ReturnNotFound_When_ClientNotExist()
        {
            this.When(x => x.WhenIRequestRemoveResource(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void RemoveResource_Should_ReturnNotContainsResource_When_ResourceNotExist()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestRemoveResource(_client.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotContainsResource))
                .BDDfy();
        }
        
        [Fact]
        public void RemoveResource_Should_ReturnNotContainsResource_When_ClientDoesNotContainsResource()
        {
            this.Given(x => x.GivenAClient()).And(x => x.GivenAResource())
                .When(x => x.WhenIRequestRemoveResource(_client.Id, _resource.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotContainsResource))
                .BDDfy();
        }
        
        [Fact]
        public void AddResource_Should_ReturnOk()
        {
            this.Given(x => x.GivenAClientWithClient())
                .When(x => x.WhenIRequestRemoveResource(_client.Id, _resource.Id))
                .Then(x => x.ThenIShouldClientWithResource())
                .BDDfy();
        }
        
        private void GivenAClient()
        {
            var request = Fixture.Create<CreateClientRequest>();
            
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            var replay = client.CreateClient(request);

            replay.Should().NotBeNull();
            replay.IsSuccess.Should().BeTrue();
            _client = replay.Value;
        }
        
        private void GivenAResource()
        {
            var request = Fixture.Build<CreateResourceRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var client = Provider.GetRequiredService<Web.Proto.Resources.ResourcesClient>();
            var replay = client.CreateResource(request);

            replay.Should().NotBeNull();
            replay.IsSuccess.Should().BeTrue();
            _resource = replay.Value;
        }
        
        private void GivenAClientWithClient()
        {
            var createClientRequest = Fixture.Create<CreateClientRequest>();
            
            var clientsClient = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            var createClientReplay = clientsClient.CreateClient(createClientRequest);

            createClientReplay.Should().NotBeNull();
            createClientReplay.IsSuccess.Should().BeTrue();

            _client = createClientReplay.Value;
            
            var createResourceRequest = Fixture.Build<CreateResourceRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var resourcesClient = Provider.GetRequiredService<Web.Proto.Resources.ResourcesClient>();
            var createResourceReplay = resourcesClient.CreateResource(createResourceRequest);

            createResourceReplay.Should().NotBeNull();
            createResourceReplay.IsSuccess.Should().BeTrue();
            
            _resource = createResourceReplay.Value;

            var addResourceReplay = clientsClient.AddResource(new AddClientResourceRequest {Id = _client.Id, ResourceId = _resource.Id});
            addResourceReplay.IsSuccess.Should().BeTrue();
        }
        
        private void WhenIRequestRemoveResource(string id, string resourceId)
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _replay = client.RemoveResource(new RemoveClientResourceRequest
            {
                Id = id,
                ResourceId = resourceId
            });
        }

        private void ThenIShouldGetError(ErrorResult error)
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeFalse();
            _replay.ErrorCode.Should().Be(error.ErrorCode);
            _replay.Description.Should().Be(error.Description);
        }
        
        private void ThenIShouldClientWithResource()
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeTrue();
            
            var provider = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            var client = provider.GetClientById(new GetClientByIdRequest {Id = _client.Id});
            client.IsSuccess.Should().BeTrue();
            
            client.Value.Resources.Should().NotContain(x => x.Id == _resource.Id);
        }
    }
}