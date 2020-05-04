using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Grpc.Core;
using IdentityServer.Acceptance.Test.Extensions;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using TestStack.BDDfy;
using Xunit;

namespace IdentityServer.Acceptance.Test.Scenes.Resources
{
    public class GetAllResource : BaseScene
    {
        private Queue<Resource> _resources;
        private AsyncServerStreamingCall<Resource> _replay;
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void CreateResourceWhenEverythingIsFine(int length)
        {
            this.Given(x => x.GivenAResource(length))
                .When(x => x.WhenIRequestGetAllResource())
                .Then(x => x.ThenIShouldGetAllCreatedResource());
        }
        
        private void GivenAResource(int length)
        {
            var client = Provider.GetRequiredService<Web.Proto.Resources.ResourcesClient>();
            _resources = new Queue<Resource>();

            for (var i = 0; i < length; i++)
            {
                var request = Fixture.Build<CreateResourceRequest>()
                    .With(x => x.Name, Fixture.CreateWithLength(20))
                    .Create();
            
            
                var replay = client.CreateResource(request);
                replay.Should().NotBeNull();
                replay.IsSuccess.Should().BeTrue();
                
                _resources.Enqueue(replay.Value);
            }
        }
        
        private void WhenIRequestGetAllResource()
        {
            var client = Provider.GetRequiredService<Web.Proto.Resources.ResourcesClient>();
            _replay = client.GetResources(new GetResourcesRequest());
            _replay.Should().NotBeNull();
        }

        private async Task ThenIShouldGetAllCreatedResource()
        {
            var stream = _replay.ResponseStream;
            
            await foreach (var resource in stream.ReadAllAsync())
            {
                _resources.TryDequeue(out var valid).Should().BeTrue();
                
                resource.Id.Should().Be(valid.Id);
                resource.Name.Should().Be(valid.Name);
                resource.DisplayName.Should().Be(valid.DisplayName);
                resource.Description.Should().Be(valid.Description);
                resource.IsEnable.Should().Be(valid.IsEnable);
            }

            _resources.Should().BeEmpty();
        }
    }
}