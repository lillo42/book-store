using AutoFixture;
using FluentAssertions;
using IdentityServer.Acceptance.Test.Extensions;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using TestStack.BDDfy;
using Xunit;

namespace IdentityServer.Acceptance.Test.Scenes.Resources
{
    public class CreateResource : BaseScene
    {
        private Resource _resource;
        private CreateResourceRequest _request;
        private CreateResourceReplay _replay;
        
        [Theory]
        [InlineData("")]
        public void NotCreateResourceWhenNameIsMissing(string name)
        {
            this.Given(x => x.GivenResourceWithName(name))
                .When(x => x.WhenCreateResourceIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.MissingName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateResourceWhenNameIsInvalid()
        {
            this.Given(x => x.GivenResourceWithName(Fixture.CreateWithLength(21)))
                .When(x => x.WhenCreateResourceIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.InvalidName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateResourceWhenDisplayIsMissing()
        {
            this.Given(x => x.GivenResourceWithName(Fixture.CreateWithLength(21)))
                .When(x => x.WhenCreateResourceIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.InvalidName))
                .BDDfy();
        }

        [Fact]
        public void NotCreateResourceWhenNameAlreadyExist()
        {
            this.Given(x => x.GivenACreatedResource())
                    .And(x=> x.GivenResourceWithName(_resource.Name))
                .When(x => x.WhenCreateResourceIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.NameAlreadyExist))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateResourceWhenDisplayIsInvalid()
        {
            this.Given(x => x.GivenUserWithDisplayName(Fixture.CreateWithLength(51)))
                .When(x => x.WhenCreateResourceIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.InvalidDisplayName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateResourceWhenDescriptionIsInvalid()
        {
            this.Given(x => x.GivenUserWithDescription(Fixture.CreateWithLength(251)))
                .When(x => x.WhenCreateResourceIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.InvalidDescription))
                .BDDfy();
        }
        
        [Fact]
        public void CreateResourceWhenEverythingIsFine()
        {
            this.Given(x => x.GivenAnValidResource())
                .When(x => x.WhenCreateResourceIsRequested())
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
        }
        
        private void GivenResourceWithName(string name)
        {
            _request = Fixture.Build<CreateResourceRequest>()
                .With(x => x.Name, name)
                .Create();
        }
        
        private void GivenACreatedResource()
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
        
        private void GivenUserWithDisplayName(string displayName)
        {
            _request = Fixture.Build<CreateResourceRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.DisplayName, displayName)
                .Create();
        }
        
        private void GivenUserWithDescription(string description)
        {
            _request = Fixture.Build<CreateResourceRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.Description, description)
                .Create();
        }
        
        private void GivenAnValidResource()
        {
            _request = Fixture.Build<CreateResourceRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
        }
        
        private void WhenCreateResourceIsRequested()
        {
            var client = Provider.GetRequiredService<Web.Proto.Resources.ResourcesClient>();
            _replay = client.CreateResource(_request);
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
            
            _replay.Value.Name.Should().NotBeNull();
            _replay.Value.Name.Should().Be(_request.Name);
            
            _replay.Value.DisplayName.Should().NotBeNull();
            _replay.Value.DisplayName.Should().Be(_request.DisplayName);
            
            _replay.Value.Description.Should().NotBeNull();
            _replay.Value.Description.Should().Be(_request.Description);
            
            _replay.Value.IsEnable.Should().Be(_request.IsEnable);
        }
    }
}