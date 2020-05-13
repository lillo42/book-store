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
    public class AddPermission : BaseScene
    {
        private User _user;
        private Permission _permission;
        private AddUserPermissionReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddPermission_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddPermission(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddPermission_Should_ReturnInvalid_When_PermissionIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddPermission(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnNotFound_When_PermissionNotExist()
        {
            this.When(x => x.WhenIRequestAddPermission(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnInvalidPermission_When_PermissionNotExist()
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestAddPermission(_user.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidPermission))
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnOk()
        {
            this.Given(x => x.GivenAUser()).And(x => x.GivenAPermission())
                .When(x => x.WhenIRequestAddPermission(_user.Id, _permission.Id))
                .Then(x => x.ThenIShouldUserWithPermission())
                .BDDfy();
        }
        
        [Fact]
        public void AddPermission_Should_ReturnPermissionAlreadyExist_When_PermissionAlreadyExist()
        {
            this.Given(x => x.GivenAUser()).And(x => x.GivenAPermission())
                .When(x => x.WhenIRequestAddPermission(_user.Id, _permission.Id)).And(x => x.WhenIRequestAddPermission(_user.Id, _permission.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.PermissionAlreadyExist))
                .BDDfy();
        }
        
        private void GivenAUser()
        {
            var request = Fixture.Build<CreateUserRequest>()
                .With(x => x.Mail, $"{Fixture.Create<string>()}@aaa.org")
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
        
        private void WhenIRequestAddPermission(string id, string permissionId)
        {
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            _replay = client.AddPermission(new AddUserPermissionRequest
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
            var user
                = client.GetUserById(new GetUserByIdRequest {Id = _user.Id});
            user.IsSuccess.Should().BeTrue();

            user.Value.Permission.Should().NotBeNullOrEmpty();
            user.Value.Permission.Should().Contain(x => x.Id == _permission.Id);
        }
    }
}