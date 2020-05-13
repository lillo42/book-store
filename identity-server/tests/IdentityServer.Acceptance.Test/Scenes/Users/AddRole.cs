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
    public class AddRole : BaseScene
    {
        private User _user;
        private Role _role;
        private AddUserRoleReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddRole_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddRole(id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidId))
                .BDDfy();
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ANC")]
        public void AddRole_Should_ReturnInvalid_When_RoleIdIsInvalid(string id)
        {
            this.When(x => x.WhenIRequestAddRole(Guid.NewGuid().ToString(), id))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void AddRole_Should_ReturnNotFound_When_RoleNotExist()
        {
            this.When(x => x.WhenIRequestAddRole(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void AddRole_Should_ReturnInvalidRole_When_RoleNotExist()
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestAddRole(_user.Id, Guid.NewGuid().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidRole))
                .BDDfy();
        }
        
        [Fact]
        public void AddRole_Should_ReturnOk()
        {
            this.Given(x => x.GivenAUser()).And(x => x.GivenARole())
                .When(x => x.WhenIRequestAddRole(_user.Id, _role.Id))
                .Then(x => x.ThenIShouldUserWithRole())
                .BDDfy();
        }
        
        [Fact]
        public void AddRole_Should_ReturnRoleAlreadyExist_When_RoleAlreadyExist()
        {
            this.Given(x => x.GivenAUser()).And(x => x.GivenARole())
                .When(x => x.WhenIRequestAddRole(_user.Id, _role.Id))
                    .And(x => x.WhenIRequestAddRole(_user.Id, _role.Id))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.RoleAlreadyExist))
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
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            _replay = client.AddRole(new AddUserRoleRequest
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
            var user
                = client.GetUserById(new GetUserByIdRequest {Id = _user.Id});
            user.IsSuccess.Should().BeTrue();

            user.Value.Roles.Should().NotBeNullOrEmpty();
            user.Value.Roles.Should().Contain(x => x.Id == _role.Id);
        }
    }
}