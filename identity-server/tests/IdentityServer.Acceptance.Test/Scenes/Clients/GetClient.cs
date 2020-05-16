using System;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using TestStack.BDDfy;
using Xunit;

namespace IdentityServer.Acceptance.Test.Scenes.Clients
{
    public class GetClient : BaseScene
    {
        private Client _client;
        private GetClientByIdReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData("ABC")]
        public void GetClient_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestGetClient(id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void GetClient_Should_ReturnNotFound_When_ClientNotFound()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestGetClient(Fixture.Create<Guid>().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void GetClient_Should_ReturnAllClient()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestGetClient(_client.Id))
                .Then(x => x.ThenIShouldGetOk())
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
        
        private void WhenIRequestGetClient(string id)
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _replay = client.GetClientById(new GetClientByIdRequest
            {
                Id = id
            });
        }

        private void ThenIShouldGetError(ErrorResult error)
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeFalse();
            _replay.ErrorCode.Should().Be(error.ErrorCode);
            _replay.Description.Should().Be(error.Description);
        }
        
        private void ThenIShouldGetOk()
        {
            _replay.Should().NotBeNull();
            
            _replay.IsSuccess.Should().BeTrue();
            _replay.ErrorCode.Should().BeNullOrEmpty();
            _replay.Description.Should().BeNullOrEmpty();
            
            _replay.Value.Id.Should().NotBeNull();
            _replay.Value.Id.Should().Be(_client.Id);
            
            _replay.Value.Name.Should().NotBeNullOrEmpty();
            _replay.Value.Name.Should().Be(_client.Name);
            
            _replay.Value.ClientId.Should().NotBeNullOrEmpty();
            _replay.Value.ClientId.Should().Be(_client.ClientId);
            
            _replay.Value.ClientSecret.Should().NotBeNullOrEmpty();
            _replay.Value.ClientSecret.Should().Be(_client.ClientSecret);
            
            _replay.Value.IsEnable.Should().Be(_client.IsEnable);
        }
    }
}