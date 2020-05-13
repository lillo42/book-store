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
    public class RemoveRole : BaseScene
    {
        private User _user;
        private Role _role;
        private RemoveUserRoleReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemoveRole_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemoveRole(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void RemoveRole_Should_ReturnInvalid_When_RoleIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestRemoveRole(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void RemoveRole_Should_ReturnNotFound_When_UserNotExist()
        {
            this.When(x => x.WhenIRequestRemoveRole(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void RemoveRole_Should_ReturnNotContainsRole_When_RoleNotExist()
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestRemoveRole(_user.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.NotContainsRole))
                .BDDfy();
        }
        
        [Fact]
        public void RemoveRole_Should_ReturnNotContainsRole_When_UserDoesNotContainsRole()
        {
            this.Given(x => x.GivenAUser()).And(x => x.GivenARole())
                .When(x => x.WhenIRequestRemoveRole(_user.Id, _role.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.NotContainsRole))
                .BDDfy();
        }
        
        [Fact]
        public void AddRole_Should_ReturnOk()
        {
            this.Given(x => x.GivenAUserWithUser())
                .When(x => x.WhenIRequestRemoveRole(_user.Id, _role.Id))
                .Then(x => x.ThenIShouldUserWithRole())
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
            
            var createRoleRequest = Fixture.Build<CreateRoleRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var rolesClient = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            var createRoleReplay = rolesClient.CreateRole(createRoleRequest);

            createRoleReplay.Should().NotBeNull();
            createRoleReplay.IsSuccess.Should().BeTrue();
            
            _role = createRoleReplay.Value;

            var addRoleReplay = usersClient.AddRole(new AddUserRoleRequest {Id = _user.Id, RoleId = _role.Id});
            addRoleReplay.IsSuccess.Should().BeTrue();
        }
        
        private void WhenIRequestRemoveRole(string id, string roleId)
        {
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            _replay = client.RemoveRole(new RemoveUserRoleRequest
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
        
        private void ThenIShouldUserWithRole()
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeTrue();
            
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            var user = client.GetUserById(new GetUserByIdRequest {Id = _user.Id});
            user.IsSuccess.Should().BeTrue();
            
            user.Value.Roles.Should().NotContain(x => x.Id == _role.Id);
        }
    }
}