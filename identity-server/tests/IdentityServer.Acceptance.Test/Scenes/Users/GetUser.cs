using System;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using TestStack.BDDfy;
using Xunit;

namespace IdentityServer.Acceptance.Test.Scenes.Users
{
    public class GetUser : BaseScene
    {
        private User _user;
        private GetUserByIdReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData("ABC")]
        public void GetUser_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestGetUser(id))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void GetUser_Should_ReturnNotFound_When_UserNotFound()
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestGetUser(Fixture.Create<Guid>().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void GetUser_Should_ReturnAllUser()
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestGetUser(_user.Id))
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
        }
        
        private void GivenAUser()
        {
            var request = Fixture.Build<CreateUserRequest>()
                .With(x => x.Mail, $"{Fixture.Create<string>()}@outlook.com")
                .Create();
            
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            var replay = client.CreateUser(request);

            replay.Should().NotBeNull();
            replay.IsSuccess.Should().BeTrue();
            _user = replay.Value;
        }
        
        private void WhenIRequestGetUser(string id)
        {
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            _replay = client.GetUserById(new GetUserByIdRequest
            {
                Id = id
            });
        }

        private void ThenIShouldGetError(ErrorResult error)
        {
            _replay.Should().NotBeNull();
            _replay.IsSuccess.Should().BeFalse();
            _replay.ErrorCode.Should().Be(error.ErrorCode);
            _replay.Description.Should().Be(error.Description);
        }
        
        private void ThenIShouldGetOk()
        {
            _replay.Should().NotBeNull();
            
            _replay.IsSuccess.Should().BeTrue();
            _replay.ErrorCode.Should().BeNullOrEmpty();
            _replay.Description.Should().BeNullOrEmpty();
            
            _replay.Value.Id.Should().NotBeNull();
            _replay.Value.Id.Should().Be(_user.Id);
            
            _replay.Value.Mail.Should().NotBeNull();
            _replay.Value.Mail.Should().Be(_user.Mail);
            
            _replay.Value.Password.Should().BeEmpty();
            
            _replay.Value.IsEnable.Should().Be(_user.IsEnable);
        }
    }
}