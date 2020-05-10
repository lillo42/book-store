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

namespace IdentityServer.Acceptance.Test.Scenes.Roles
{
    public class AddPermission : BaseScene
    {
        private Role _role;
        private Permission _permission;
        private AddPermissionReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddPermission_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddPermission(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddPermission_Should_ReturnInvalid_When_PermissionIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddPermission(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnNotFound_When_PermissionNotExist()
        {
            this.When(x => x.WhenIRequestAddPermission(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnInvalidPermission_When_PermissionNotExist()
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestAddPermission(_role.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidPermission))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnOk()
        {
            this.Given(x => x.GivenARole()).And(x => x.GivenAPermission())
                .When(x => x.WhenIRequestAddPermission(_role.Id, _permission.Id))
                .Then(x => x.ThenIShouldRoleWithPermission())
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnPermissionAlreadyExist_When_PermissionAlreadyExist()
        {
            this.Given(x => x.GivenARole()).And(x => x.GivenAPermission())
                .When(x => x.WhenIRequestAddPermission(_role.Id, _permission.Id)).And(x => x.WhenIRequestAddPermission(_role.Id, _permission.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.PermissionAlreadyExist))
                .BDDfy();
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
            var client = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            _replay = client.AddPermission(new AddPermissionRequest
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
        
        private void ThenIShouldRoleWithPermission()
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeTrue();
            
            var client = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            var role = client.GetRoleById(new GetRoleByIdRequest {Id = _role.Id});
            role.IsSuccess.Should().BeTrue();

            role.Value.Permission.Should().NotBeNullOrEmpty();
            role.Value.Permission.Should().Contain(x => x.Id == _permission.Id);
        }
    }
}