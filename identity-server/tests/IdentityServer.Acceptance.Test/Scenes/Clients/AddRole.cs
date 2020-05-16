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
    public class AddRole : BaseScene
    {
        private Client _client;
        private Role _role;
        private AddClientRoleReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddRole_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddRole(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddRole_Should_ReturnInvalid_When_RoleIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddRole(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void AddRole_Should_ReturnNotFound_When_RoleNotExist()
        {
            this.When(x => x.WhenIRequestAddRole(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void AddRole_Should_ReturnInvalidRole_When_RoleNotExist()
        {
            this.Given(x => x.GivenAClient())
                .When(x => x.WhenIRequestAddRole(_client.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.InvalidRole))
                .BDDfy();
        }
        
        [Fact]
        public void AddRole_Should_ReturnOk()
        {
            this.Given(x => x.GivenAClient()).And(x => x.GivenARole())
                .When(x => x.WhenIRequestAddRole(_client.Id, _role.Id))
                .Then(x => x.ThenIShouldClientWithRole())
                .BDDfy();
        }
        
        [Fact]
        public void AddRole_Should_ReturnRoleAlreadyExist_When_RoleAlreadyExist()
        {
            this.Given(x => x.GivenAClient()).And(x => x.GivenARole())
                .When(x => x.WhenIRequestAddRole(_client.Id, _role.Id))
                    .And(x => x.WhenIRequestAddRole(_client.Id, _role.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.ClientError.RoleAlreadyExist))
                .BDDfy();
        }
        
        private void GivenAClient()
        {
            var request = Fixture.Build<CreateClientRequest>()
                .Create();
            
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
        
        private void WhenIRequestAddRole(string id, string roleId)
        {
            var client = Provider.GetRequiredService<Web.Proto.Clients.ClientsClient>();
            _replay = client.AddRole(new AddClientRoleRequest
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

            client.Value.Roles.Should().NotBeNullOrEmpty();
            client.Value.Roles.Should().Contain(x => x.Id == _role.Id);
        }
    }
}