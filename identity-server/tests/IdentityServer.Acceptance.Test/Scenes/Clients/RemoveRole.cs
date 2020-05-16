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

namespace IdentityServer.Acceptance.Test.Scenes.Clients
{
    public class RemoveRole : BaseScene
    {
        private Client _client;
        private Role _role;
        private RemoveClientRoleReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemoveRole_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemoveRole(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemoveRole_Should_ReturnInvalid_When_RoleIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemoveRole(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void RemoveRole_Should_ReturnNotFound_When_ClientNotExist()
        {
            this.When(x => x.WhenIRequestRemoveRole(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void RemoveRole_Should_ReturnNotContainsRole_When_RoleNotExist()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestRemoveRole(_client.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotContainsRole))
                .BDDfy();
        }
        
        [Fact]
        public void RemoveRole_Should_ReturnNotContainsRole_When_ClientDoesNotContainsRole()
        {
            this.Given(x => x.GivenAClient()).And(x => x.GivenARole())
                .When(x => x.WhenIRequestRemoveRole(_client.Id, _role.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotContainsRole))
                .BDDfy();
        }
        
        [Fact]
        public void AddRole_Should_ReturnOk()
        {
            this.Given(x => x.GivenAClientWithClient())
                .When(x => x.WhenIRequestRemoveRole(_client.Id, _role.Id))
                .Then(x => x.ThenIShouldClientWithRole())
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
        
        private void GivenARole()
        {
            var request = Fixture.Build<CreateRoleRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var client = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            var replay = client.CreateRole(request);

            replay.Should().NotBeNull();
            replay.IsSuccess.Should().BeTrue();
            _role = replay.Value;
        }
        
        private void GivenAClientWithClient()
        {
            var createClientRequest = Fixture.Create<CreateClientRequest>();
            
            var clientsClient = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            var createClientReplay = clientsClient.CreateClient(createClientRequest);

            createClientReplay.Should().NotBeNull();
            createClientReplay.IsSuccess.Should().BeTrue();

            _client = createClientReplay.Value;
            
            var createRoleRequest = Fixture.Build<CreateRoleRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var rolesClient = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            var createRoleReplay = rolesClient.CreateRole(createRoleRequest);

            createRoleReplay.Should().NotBeNull();
            createRoleReplay.IsSuccess.Should().BeTrue();
            
            _role = createRoleReplay.Value;

            var addRoleReplay = clientsClient.AddRole(new AddClientRoleRequest {Id = _client.Id, RoleId = _role.Id});
            addRoleReplay.IsSuccess.Should().BeTrue();
        }
        
        private void WhenIRequestRemoveRole(string id, string roleId)
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _replay = client.RemoveRole(new RemoveClientRoleRequest
            {
                Id = id,
                RoleId = roleId
            });
        }

        private void ThenIShouldGetError(ErrorResult error)
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeFalse();
            _replay.ErrorCode.Should().Be(error.ErrorCode);
            _replay.Description.Should().Be(error.Description);
        }
        
        private void ThenIShouldClientWithRole()
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeTrue();
            
            var provider = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            var client = provider.GetClientById(new GetClientByIdRequest {Id = _client.Id});
            client.IsSuccess.Should().BeTrue();
            
            client.Value.Roles.Should().NotContain(x => x.Id == _role.Id);
        }
    }
}