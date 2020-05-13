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

namespace IdentityServer.Acceptance.Test.Scenes.Users
{
    public class RemovePermission : BaseScene
    {
        private User _user;
        private Permission _permission;
        private RemoveUserPermissionReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemovePermission_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemovePermission(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemovePermission_Should_ReturnInvalid_When_PermissionIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemovePermission(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void RemovePermission_Should_ReturnNotFound_When_UserNotExist()
        {
            this.When(x => x.WhenIRequestRemovePermission(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void RemovePermission_Should_ReturnNotContainsPermission_When_PermissionNotExist()
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestRemovePermission(_user.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.NotContainsPermission))
                .BDDfy();
        }
        
        [Fact]
        public void RemovePermission_Should_ReturnNotContainsPermission_When_UserDoesNotContainsPermission()
        {
            this.Given(x => x.GivenAUser()).And(x => x.GivenAPermission())
                .When(x => x.WhenIRequestRemovePermission(_user.Id, _permission.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.NotContainsPermission))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnOk()
        {
            this.Given(x => x.GivenAUserWithUser())
                .When(x => x.WhenIRequestRemovePermission(_user.Id, _permission.Id))
                .Then(x => x.ThenIShouldUserWithPermission())
                .BDDfy();
        }
        
        private void GivenAUser()
        {
            var request = Fixture.Build<CreateUserRequest>()
                .With(x => x.Mail, $"{Fixture.Create<string>()}@info.com")
                .Create();
            
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            var replay = client.CreateUser(request);

            replay.Should().NotBeNull();
            replay.IsSuccess.Should().BeTrue();
            _user = replay.Value;
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
        
        private void GivenAUserWithUser()
        {
            var createUserRequest = Fixture.Build<CreateUserRequest>()
                .With(x => x.Mail, $"{Fixture.Create<string>()}@info.com")
                .Create();
            
            var usersClient = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            var createUserReplay = usersClient.CreateUser(createUserRequest);

            createUserReplay.Should().NotBeNull();
            createUserReplay.IsSuccess.Should().BeTrue();

            _user = createUserReplay.Value;
            
            var createPermissionRequest = Fixture.Build<CreatePermissionRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var permissionsClient = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            var createPermissionReplay = permissionsClient.CreatePermission(createPermissionRequest);

            createPermissionReplay.Should().NotBeNull();
            createPermissionReplay.IsSuccess.Should().BeTrue();
            
            _permission = createPermissionReplay.Value;

            var addPermissionReplay = usersClient.AddPermission(new AddUserPermissionRequest {Id = _user.Id, PermissionId = _permission.Id});
            addPermissionReplay.IsSuccess.Should().BeTrue();
        }
        
        private void WhenIRequestRemovePermission(string id, string permissionId)
        {
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            _replay = client.RemovePermission(new RemoveUserPermissionRequest
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
        
        private void ThenIShouldUserWithPermission()
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeTrue();
            
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            var user = client.GetUserById(new GetUserByIdRequest {Id = _user.Id});
            user.IsSuccess.Should().BeTrue();
            
            user.Value.Permission.Should().NotContain(x => x.Id == _permission.Id);
        }
    }
}