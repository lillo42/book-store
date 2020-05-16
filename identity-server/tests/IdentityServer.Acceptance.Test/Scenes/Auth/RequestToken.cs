using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Acceptance.Test.Extensions;
using IdentityServer.Web.Proto;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steeltoe.Common.Http;
using TestStack.BDDfy;
using Xunit;

namespace IdentityServer.Acceptance.Test.Scenes.Auth
{
    public class RequestToken : BaseScene
    {
        private Web.Proto.Client _client;
        private HttpResponseMessage _message;

        [Fact]
        public void RequestToken_Should_ReturnToken()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestToken(_client.ClientId, _client.ClientSecret, GrantType.ClientCredentials))
                .Then(x => x.ThenIShouldGetTheToken())
                .BDDfy();
        }

        private void GivenAClient()
        {
            #region Permission
            var requestPermission = Fixture.Build<CreatePermissionRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            var permissionClient = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            var permission = permissionClient.CreatePermission(requestPermission);
            permission.IsSuccess.Should().BeTrue();
            #endregion
            
            #region Role
            var requestRole = Fixture.Build<CreateRoleRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            var roleClient = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            var role = roleClient.CreateRole(requestRole);
            role.IsSuccess.Should().BeTrue();
            #endregion
            
            #region Resource
            var requestResource = Fixture.Build<CreateResourceRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            var resourceClient = Provider.GetRequiredService<Web.Proto.Resources.ResourcesClient>();
            var resource = resourceClient.CreateResource(requestResource);
            resource.IsSuccess.Should().BeTrue();
            #endregion
            
            #region Client
            var requestClient = Fixture.Create<CreateClientRequest>();
            var clientClient = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            var client = clientClient.CreateClient(requestClient);
            client.IsSuccess.Should().BeTrue();

            var permissionAdd = clientClient.AddPermission(new AddClientPermissionRequest
            {
                Id = client.Value.Id,
                PermissionId = permission.Value.Id
            });
            
            permissionAdd.IsSuccess.Should().BeTrue();
            
            var roleAdd = clientClient.AddRole(new AddClientRoleRequest
            {
                Id = client.Value.Id,
                RoleId = role.Value.Id
            });
            
            roleAdd.IsSuccess.Should().BeTrue();
            
            var resourceAdd = clientClient.AddResource(new AddClientResourceRequest
            {
                Id = client.Value.Id,
                ResourceId = resource.Value.Id
            });
            
            resourceAdd.IsSuccess.Should().BeTrue();

            _client = client.Value;
            #endregion

        }

        private async Task WhenIRequestToken(string clientId, string clientSecret, string grantType)
        {
            var http = Provider.GetRequiredService<HttpClient>();
            _message = await http.PostAsync("/connect/token",
                new StringContent(JsonConvert.SerializeObject(new RequestTokenDTO
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    GrantType = grantType
                }), Encoding.UTF8, "application/json"));
        }

        private async Task ThenIShouldGetTheToken()
        {
            _message.IsSuccessStatusCode.Should().BeTrue();
            var parse = JsonConvert.DeserializeObject<JToken>(await _message.Content.ReadAsStringAsync());
        }
        
    }

    public class RequestTokenDTO
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
        
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
    }
}