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
    public class UpdateClient : BaseScene
    {
        private Client _client;
        private UpdateClientRequest _request;
        private UpdateClientReplay _replay;
        
        [Theory]
        [InlineData("")]
        public void UpdateClient_Should_ReturnMissingName_When_NameIsMissing(string name)
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestUpdateWithName(name))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.MissingName))
                .BDDfy();
        }
        
        [Fact]
        public void UpdateClient_Should_ReturnInvalidName_When_NameIsGreaterThan101()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestUpdateWithName(Fixture.CreateWithLength(101)))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidName))
                .BDDfy();
        }
        
        [Fact]
        public void UpdateClient_Should_ReturnNameAlreadyExist_When_NameAlreadyExist()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestUpdateWithName(_client.Name))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NameAlreadyExist));
        }
        
        [Theory]
        [InlineData("")]
        public void UpdateClient_Should_ReturnMissingClientId_When_ClientIdIsMissing(string name)
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestUpdateWithClientId(name))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.MissingClientId))
                .BDDfy();
        }
        
        [Fact]
        public void UpdateClient_Should_ReturnInvalidClientId_When_ClientIdIsGreaterThan50()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestUpdateWithClientId(Fixture.CreateWithLength(51)))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidClientId))
                .BDDfy();
        }
        
        [Fact]
        public void UpdateClient_Should_ReturnClientIdAlreadyExist_When_ClientIdAlreadyExist()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestUpdateWithClientId(_client.ClientId))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.ClientIdAlreadyExist));
        }
        
        [Theory]
        [InlineData("")]
        public void UpdateClient_Should_ReturnMissingClientSecret_When_ClientSecretIsMissing(string name)
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestUpdateWithClientSecret(name))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.MissingClientSecret))
                .BDDfy();
        }
        
        [Fact]
        public void UpdateClient_Should_ReturnInvalidClientSecret_When_ClientSecretIsGreaterThan250()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestUpdateWithClientSecret(Fixture.CreateWithLength(251)))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidClientSecret))
                .BDDfy();
        }

        [Fact]
        public void UpdateClient_Should_ReturnOk()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestUpdate())
                .Then(x => x.ThenIShouldGetOk())
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

        private void WhenIRequestUpdateWithName(string name)
        {
            _request = Fixture.Build<UpdateClientRequest>()
                .With(x => x.Id, _client.Id)
                .With(x => x.Name, name)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithClientId(string clientId)
        {
            _request = Fixture.Build<UpdateClientRequest>()
                .With(x => x.Id, _client.Id)
                .With(x => x.ClientId, clientId)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithClientSecret(string clientSecret)
        {
            _request = Fixture.Build<UpdateClientRequest>()
                .With(x => x.Id, _client.Id)
                .With(x => x.ClientSecret, clientSecret)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdate()
        {
            _request = Fixture.Build<UpdateClientRequest>()
                .With(x => x.Id, _client.Id)
                .Create();
            
            ExecuteRequest();
        }
        
        private void ExecuteRequest()
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _replay = client.UpdateClient(_request);
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
            
            _replay.Value.Name.Should().NotBeNullOrEmpty();
            _replay.Value.Name.Should().Be(_request.Name);
            
            _replay.Value.ClientId.Should().NotBeNullOrEmpty();
            _replay.Value.ClientId.Should().Be(_request.ClientId);
            
            _replay.Value.ClientSecret.Should().NotBeNullOrEmpty();
            _replay.Value.ClientSecret.Should().Be(_request.ClientSecret);
            
            _replay.Value.IsEnable.Should().Be(_request.IsEnable);
        }
    }
}