using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Resource> _resources;
        private AsyncServerStreamingCall<Resource> _replay;
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void CreateResourceWhenEverythingIsFine(int length)
        {
            this.Given(x => x.GivenAResource(length))
                .When(x => x.WhenIRequestGetAllResource())
                .Then(x => x.ThenIShouldGetAllCreatedResource())
                .BDDfy();
        }
        
        private void GivenAResource(int length)
        {
            var client = Provider.GetRequiredService<Web.Proto.Resources.ResourcesClient>();
            _resources = new List<Resource>(length);

            for (var i = 0; i < length; i++)
            {
                var request = Fixture.Build<CreateResourceRequest>()
                    .With(x => x.Name, Fixture.CreateWithLength(20))
                    .Create();
            
            
                var replay = client.CreateResource(request);
                replay.Should().NotBeNull();
                replay.IsSuccess.Should().BeTrue();
                
                _resources.Add(replay.Value);
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
                var compared = _resources.FirstOrDefault(x => x.Id.Equals(resource.Id, StringComparison.InvariantCultureIgnoreCase));

                if (compared == null)
                {
                    continue;
                }
                
                resource.Id.Should().Be(compared.Id);
                resource.Name.Should().Be(compared.Name);
                resource.DisplayName.Should().Be(compared.DisplayName);
                resource.Description.Should().Be(compared.Description);
                resource.IsEnable.Should().Be(compared.IsEnable);
                
                _resources.Remove(compared);
            }

            _resources.Should().BeEmpty();
        }
    }
}