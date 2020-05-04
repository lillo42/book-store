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
    public class UpdateResource : BaseScene
    {
        private Resource _resource;
        private UpdateResourceRequest _request;
        private UpdateResourceReplay _replay;
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void NotCreateResourceWhenNameIsMissing(string name)
        {
            this.Given(x => x.GivenAResource())
                .When(x => x.WhenIRequestUpdateWithName(name))
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.MissingName));
        }
        
        [Fact]
        public void NotCreateResourceWhenNameIsInvalid()
        {
            this.Given(x => x.GivenAResource())
                .When(x => x.WhenIRequestUpdateWithName(Fixture.CreateWithLength(21)))
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.InvalidName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void NotCreateResourceWhenDisplayIsMissing(string displayName)
        {
            this.Given(x => x.GivenAResource())
                .When(x => x.WhenIRequestUpdateWithDisplayName(displayName))
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.MissingDisplayName));
        }
        
        [Fact]
        public void NotCreateResourceWhenDisplayIsInvalid()
        {
            this.Given(x => x.GivenAResource())
                .When(x => x.WhenIRequestUpdateWithDisplayName(Fixture.CreateWithLength(51)))
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.InvalidDisplayName));
        }
        
        [Fact]
        public void NotCreateResourceWhenDescriptionIsInvalid()
        {
            this.Given(x => x.GivenAResource())
                .When(x => x.WhenIRequestUpdateWithDescription(Fixture.CreateWithLength(251)))
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.InvalidDescription));
        }
        
        [Fact]
        public void CreateResourceWhenEverythingIsFine()
        {
            this.Given(x => x.GivenAResource())
                .When(x => x.WhenIRequestUpdateWithValidResource())
                .Then(x => x.ThenIShouldGetOk());
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

        private void WhenIRequestUpdateWithName(string name)
        {
            _request = Fixture.Build<UpdateResourceRequest>()
                .With(x => x.Id, _resource.Id)
                .With(x => x.Name, name)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithDisplayName(string displayName)
        {
            _request = Fixture.Build<UpdateResourceRequest>()
                .With(x => x.Id, _resource.Id)
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.DisplayName, displayName)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithDescription(string description)
        {
            _request = Fixture.Build<UpdateResourceRequest>()
                .With(x => x.Id, _resource.Id)
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.Description, description)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithValidResource()
        {
            _request = Fixture.Build<UpdateResourceRequest>()
                .With(x => x.Id, _resource.Id)
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            ExecuteRequest();
        }
        
        private void ExecuteRequest()
        {
            var client = Provider.GetRequiredService<Web.Proto.Resources.ResourcesClient>();
            _replay = client.UpdateResource(_request);
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