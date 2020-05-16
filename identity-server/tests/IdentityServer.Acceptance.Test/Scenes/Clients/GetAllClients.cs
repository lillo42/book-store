using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Grpc.Core;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using TestStack.BDDfy;
using Xunit;

namespace IdentityServer.Acceptance.Test.Scenes.Clients
{
    public class GetAllClients : BaseScene
    {
        private List<Client> _clients;
        private AsyncServerStreamingCall<Client> _replay;
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void GetClients_Should_ReturnOk(int length)
        {
            this.Given(x => x.GivenAClient(length))
                .When(x => x.WhenIRequestGetAllClient())
                .Then(x => x.ThenIShouldGetAllCreatedClient())
                .BDDfy();
        }
        
        private void GivenAClient(int length)
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _clients = new List<Client>(length);

            for (var i = 0; i < length; i++)
            {
                var request = Fixture.Create<CreateClientRequest>();

                var replay = client.CreateClient(request);
                replay.Should().NotBeNull();
                replay.IsSuccess.Should().BeTrue();
                
                _clients.Add(replay.Value);
            }
        }
        
        private void WhenIRequestGetAllClient()
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _replay = client.GetClients(new GetClientsRequest());
            _replay.Should().NotBeNull();
        }

        private async Task ThenIShouldGetAllCreatedClient()
        {
            var stream = _replay.ResponseStream;
            
            await foreach (var client in stream.ReadAllAsync())
            {
                var compared = _clients.FirstOrDefault(x => x.Id.Equals(client.Id, StringComparison.InvariantCultureIgnoreCase));

                if (compared == null)
                {
                    continue;
                }
                
                client.Id.Should().Be(compared.Id);
                client.Name.Should().Be(compared.Name);
                client.ClientId.Should().Be(compared.ClientId);
                client.ClientSecret.Should().Be(compared.ClientSecret);
                client.IsEnable.Should().Be(compared.IsEnable);
                
                _clients.Remove(compared);
            }

            _clients.Should().BeEmpty();
        }
    }
}