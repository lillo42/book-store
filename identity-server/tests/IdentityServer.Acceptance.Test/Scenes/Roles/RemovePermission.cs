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
    public class RemovePermission : BaseScene
    {
        private Role _role;
        private Permission _permission;
        private RemovePermissionReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemovePermission_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemovePermission(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemovePermission_Should_ReturnInvalid_When_PermissionIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemovePermission(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void RemovePermission_Should_ReturnNotFound_When_RoleNotExist()
        {
            this.When(x => x.WhenIRequestRemovePermission(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void RemovePermission_Should_ReturnNotContainsPermission_When_PermissionNotExist()
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestRemovePermission(_role.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.NotContainsPermission))
                .BDDfy();
        }
        
        [Fact]
        public void RemovePermission_Should_ReturnNotContainsPermission_When_RoleDoesNotContainsPermission()
        {
            this.Given(x => x.GivenARole()).And(x => x.GivenAPermission())
                .When(x => x.WhenIRequestRemovePermission(_role.Id, _permission.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.NotContainsPermission))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnOk()
        {
            this.Given(x => x.GivenARoleWithRole())
                .When(x => x.WhenIRequestRemovePermission(_role.Id, _permission.Id))
                .Then(x => x.ThenIShouldRoleWithPermission())
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
        
        private void GivenARoleWithRole()
        {
            var createRoleRequest = Fixture.Build<CreateRoleRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var rolesClient = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            var createRoleReplay = rolesClient.CreateRole(createRoleRequest);

            createRoleReplay.Should().NotBeNull();
            createRoleReplay.IsSuccess.Should().BeTrue();

            _role = createRoleReplay.Value;
            
            var createPermissionRequest = Fixture.Build<CreatePermissionRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var permissionsClient = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            var createPermissionReplay = permissionsClient.CreatePermission(createPermissionRequest);

            createPermissionReplay.Should().NotBeNull();
            createPermissionReplay.IsSuccess.Should().BeTrue();
            
            _permission = createPermissionReplay.Value;

            var addPermissionReplay = rolesClient.AddPermission(new AddPermissionRequest {Id = _role.Id, PermissionId = _permission.Id});
            addPermissionReplay.IsSuccess.Should().BeTrue();
        }
        
        private void WhenIRequestRemovePermission(string id, string permissionId)
        {
            var client = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            _replay = client.RemovePermission(new RemovePermissionRequest
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
            
            role.Value.Permission.Should().NotContain(x => x.Id == _permission.Id);
        }
    }
}