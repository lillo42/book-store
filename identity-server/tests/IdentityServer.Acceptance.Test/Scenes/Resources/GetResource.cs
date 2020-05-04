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

namespace IdentityServer.Acceptance.Test.Scenes.Resources
{
    public class GetResource : BaseScene
    {
        private Resource _resource;
        private GetResourceByIeReplay _replay;
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ABC")]
        public void GetResource_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.Given(x => x.GivenAResource())
                .When(x => x.WhenIRequestGetResource(id))
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.InvalidId));
        }
        
        [Fact]
        public void GetResource_Should_ReturnNotFound_When_ResourceNotFound()
        {
            this.Given(x => x.GivenAResource())
                .When(x => x.WhenIRequestGetResource(Fixture.Create<Guid>().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ResourceError.NotFound));
        }
        
        [Fact]
        public void CreateResourceWhenEverythingIsFine()
        {
            this.Given(x => x.GivenAResource())
                .When(x => x.WhenIRequestGetResource(_resource.Id))
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
        
        private void WhenIRequestGetResource(string id)
        {
            var client = Provider.GetRequiredService<Web.Proto.Resources.ResourcesClient>();
            _replay = client.GetResourceById(new GetResourceByIdRequest
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
            _replay.Value.Id.Should().Be(_resource.Id);
            
            _replay.Value.Name.Should().NotBeNull();
            _replay.Value.Name.Should().Be(_resource.Name);
            
            _replay.Value.DisplayName.Should().NotBeNull();
            _replay.Value.DisplayName.Should().Be(_resource.DisplayName);
            
            _replay.Value.Description.Should().NotBeNull();
            _replay.Value.Description.Should().Be(_resource.Description);
            
            _replay.Value.IsEnable.Should().Be(_resource.IsEnable);
        }
    }
}