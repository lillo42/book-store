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
    public class CreateClient : BaseScene
    {
        private Client _user;
        private CreateClientRequest _request;
        private CreateClientReplay _replay;

        [Theory]
        [InlineData("")]
        public void CreateClient_Should_ReturnMissingName_When_NameIsMissing(string name)
        {
            this.Given(x => x.GivenClientWithName(name))
                .When(x => x.WhenCreateClientIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.MissingName))
                .BDDfy();
        }
        
        [Fact]
        public void CreateClient_Should_ReturnInvalidName_When_NameIsGreaterThan100()
        {
            this.Given(x => x.GivenClientWithName(Fixture.CreateWithLength(101)))
                .When(x => x.WhenCreateClientIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidName))
                .BDDfy();
        }
        
        [Fact]
        public void CreateClient_Should_NameAlreadyExist_When_NameAlreadyExist()
        {
            this.Given(x => x.GivenACreatedUser())
                .And(x=> x.GivenClientWithName(_user.Name))
                .When(x => x.WhenCreateClientIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NameAlreadyExist))
                .BDDfy();
        }

        [Theory]
        [InlineData("")]
        public void CreateClient_Should_ReturnMissingClientId_When_ClientIdIsMissing(string clientId)
        {
            this.Given(x => x.GivenClientWithClientId(clientId))
                .When(x => x.WhenCreateClientIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.MissingClientId))
                .BDDfy();
        }
        
        [Fact]
        public void CreateClient_Should_ReturnInvalidClientId_When_ClientIdIsGreaterThan50()
        {
            this.Given(x => x.GivenClientWithClientId(Fixture.CreateWithLength(51)))
                .When(x => x.WhenCreateClientIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidClientId))
                .BDDfy();
        }
        
        [Fact]
        public void CreateClient_Should_ClientIdAlreadyExist_When_ClientIdAlreadyExist()
        {
            this.Given(x => x.GivenACreatedUser())
                .And(x=> x.GivenClientWithClientId(_user.ClientId))
                .When(x => x.WhenCreateClientIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NameAlreadyExist))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        public void CreateClient_Should_ReturnMissingClientSecret_When_ClientSecretIsMissing(string clientSecret)
        {
            this.Given(x => x.GivenClientWithClientSecret(clientSecret))
                .When(x => x.WhenCreateClientIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.MissingClientSecret))
                .BDDfy();
        }
        
        [Fact]
        public void CreateClient_Should_ReturnInvalidClientSecret_When_ClientSecretIsGreaterThan250()
        {
            this.Given(x => x.GivenClientWithClientSecret(Fixture.CreateWithLength(251)))
                .When(x => x.WhenCreateClientIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidClientSecret))
                .BDDfy();
        }
        
        [Fact]
        public void CreateClient_Should_ReturnOk()
        {
            this.Given(x => x.GivenAnValidClient())
                .When(x => x.WhenCreateClientIsRequested())
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
        }
        
        private void GivenClientWithName(string name)
        {
            _request = Fixture.Build<CreateClientRequest>()
                .With(x => x.Name, name)
                .Create();
        }
        
        private void GivenACreatedUser()
        {
            var request = Fixture.Build<CreateClientRequest>()
                .Create();
            
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            var replay = client.CreateClient(request);

            replay.Should().NotBeNull();
            replay.IsSuccess.Should().BeTrue();

            _user = replay.Value;
        }
        
        private void GivenClientWithClientId(string clientId)
        {
            _request = Fixture.Build<CreateClientRequest>()
                .With(x => x.ClientId, clientId)
                .Create();
        }
        
        private void GivenClientWithClientSecret(string clientSecret)
        {
            _request = Fixture.Build<CreateClientRequest>()
                .With(x => x.ClientSecret, clientSecret)
                .Create();
        }
        
        
        private void GivenAnValidClient()
        {
            _request = Fixture.Create<CreateClientRequest>();
        }
        
        private void WhenCreateClientIsRequested()
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _replay = client.CreateClient(_request);
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