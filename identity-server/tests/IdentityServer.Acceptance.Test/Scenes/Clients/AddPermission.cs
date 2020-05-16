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
    public class AddPermission : BaseScene
    {
        private Client _client;
        private Permission _permission;
        private AddClientPermissionReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddPermission_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddPermission(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddPermission_Should_ReturnInvalid_When_PermissionIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddPermission(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnNotFound_When_PermissionNotExist()
        {
            this.When(x => x.WhenIRequestAddPermission(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnInvalidPermission_When_PermissionNotExist()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestAddPermission(_client.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidPermission))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnOk()
        {
            this.Given(x => x.GivenAClient()).And(x => x.GivenAPermission())
                .When(x => x.WhenIRequestAddPermission(_client.Id, _permission.Id))
                .Then(x => x.ThenIShouldClientWithPermission())
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnPermissionAlreadyExist_When_PermissionAlreadyExist()
        {
            this.Given(x => x.GivenAClient()).And(x => x.GivenAPermission())
                .When(x => x.WhenIRequestAddPermission(_client.Id, _permission.Id))
                    .And(x => x.WhenIRequestAddPermission(_client.Id, _permission.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.PermissionAlreadyExist))
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
        
        private void WhenIRequestAddPermission(string id, string permissionId)
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _replay = client.AddPermission(new AddClientPermissionRequest
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
            
            var resolver = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            var client = resolver.GetClientById(new GetClientByIdRequest {Id = _client.Id});
            client.IsSuccess.Should().BeTrue();

            client.Value.Permissions.Should().NotBeNullOrEmpty();
            client.Value.Permissions.Should().Contain(x => x.Id == _permission.Id);
        }
    }
}