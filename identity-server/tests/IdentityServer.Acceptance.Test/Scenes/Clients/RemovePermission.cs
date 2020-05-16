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
    public class RemovePermission : BaseScene
    {
        private Client _client;
        private Permission _permission;
        private RemoveClientPermissionReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemovePermission_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemovePermission(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemovePermission_Should_ReturnInvalid_When_PermissionIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemovePermission(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void RemovePermission_Should_ReturnNotFound_When_ClientNotExist()
        {
            this.When(x => x.WhenIRequestRemovePermission(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void RemovePermission_Should_ReturnNotContainsPermission_When_PermissionNotExist()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestRemovePermission(_client.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotContainsPermission))
                .BDDfy();
        }
        
        [Fact]
        public void RemovePermission_Should_ReturnNotContainsPermission_When_ClientDoesNotContainsPermission()
        {
            this.Given(x => x.GivenAClient()).And(x => x.GivenAPermission())
                .When(x => x.WhenIRequestRemovePermission(_client.Id, _permission.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotContainsPermission))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnOk()
        {
            this.Given(x => x.GivenAClientWithClient())
                .When(x => x.WhenIRequestRemovePermission(_client.Id, _permission.Id))
                .Then(x => x.ThenIShouldClientWithPermission())
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
        
        private void GivenAPermission()
        {
            var request = Fixture.Build<CreatePermissionRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var client = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            var replay = client.CreatePermission(request);

            replay.Should().NotBeNull();
            replay.IsSuccess.Should().BeTrue();
            _permission = replay.Value;
        }
        
        private void GivenAClientWithClient()
        {
            var createClientRequest = Fixture.Create<CreateClientRequest>();
            
            var clientsClient = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            var createClientReplay = clientsClient.CreateClient(createClientRequest);

            createClientReplay.Should().NotBeNull();
            createClientReplay.IsSuccess.Should().BeTrue();

            _client = createClientReplay.Value;
            
            var createPermissionRequest = Fixture.Build<CreatePermissionRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var permissionsClient = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            var createPermissionReplay = permissionsClient.CreatePermission(createPermissionRequest);

            createPermissionReplay.Should().NotBeNull();
            createPermissionReplay.IsSuccess.Should().BeTrue();
            
            _permission = createPermissionReplay.Value;

            var addPermissionReplay = clientsClient.AddPermission(new AddClientPermissionRequest {Id = _client.Id, PermissionId = _permission.Id});
            addPermissionReplay.IsSuccess.Should().BeTrue();
        }
        
        private void WhenIRequestRemovePermission(string id, string permissionId)
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _replay = client.RemovePermission(new RemoveClientPermissionRequest
            {
                Id = id,
                PermissionId = permissionId
            });
        }

        private void ThenIShouldGetError(ErrorResult error)
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeFalse();
            _replay.ErrorCode.Should().Be(error.ErrorCode);
            _replay.Description.Should().Be(error.Description);
        }
        
        private void ThenIShouldClientWithPermission()
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeTrue();
            
            var provider = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            var client = provider.GetClientById(new GetClientByIdRequest {Id = _client.Id});
            client.IsSuccess.Should().BeTrue();
            
            client.Value.Permissions.Should().NotContain(x => x.Id == _permission.Id);
        }
    }
}