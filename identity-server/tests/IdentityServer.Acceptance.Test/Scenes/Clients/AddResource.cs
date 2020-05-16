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
    public class AddResource : BaseScene
    {
        private Client _client;
        private Resource _resource;
        private AddClientResourceReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddResource_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddResource(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddResource_Should_ReturnInvalid_When_ResourceIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddResource(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void AddResource_Should_ReturnNotFound_When_ResourceNotExist()
        {
            this.When(x => x.WhenIRequestAddResource(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void AddResource_Should_ReturnInvalidResource_When_ResourceNotExist()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestAddResource(_client.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidResource))
                .BDDfy();
        }
        
        [Fact]
        public void AddResource_Should_ReturnOk()
        {
            this.Given(x => x.GivenAClient()).And(x => x.GivenAResource())
                .When(x => x.WhenIRequestAddResource(_client.Id, _resource.Id))
                .Then(x => x.ThenIShouldClientWithResource())
                .BDDfy();
        }
        
        [Fact]
        public void AddResource_Should_ReturnResourceAlreadyExist_When_ResourceAlreadyExist()
        {
            this.Given(x => x.GivenAClient()).And(x => x.GivenAResource())
                .When(x => x.WhenIRequestAddResource(_client.Id, _resource.Id))
                    .And(x => x.WhenIRequestAddResource(_client.Id, _resource.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.ResourceAlreadyExist))
                .BDDfy();
        }
        
        private void GivenAClient()
        {
            var request = Fixture.Build<CreateClientRequest>()
                .Create();
            
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
        
        private void WhenIRequestAddResource(string id, string ResourceId)
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _replay = client.AddResource(new AddClientResourceRequest
            {
                Id = id,
                ResourceId = ResourceId
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

            client.Value.Resources.Should().NotBeNullOrEmpty();
            client.Value.Resources.Should().Contain(x => x.Id == _resource.Id);
        }
    }
}