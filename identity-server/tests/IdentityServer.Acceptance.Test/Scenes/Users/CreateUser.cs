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
    public class CreateUser : BaseScene
    {
        private User _user;
        private CreateUserRequest _request;
        private CreateUserReplay _replay;
        
        [Theory]
        [InlineData("")]
        public void CreateUser_Should_ReturnMissingMail_When_MailIsMissing(string name)
        {
            this.Given(x => x.GivenUserWithMail(name))
                .When(x => x.WhenCreateUserIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.MissingMail))
                .BDDfy();
        }
        
        [Fact]
        public void CreateUser_Should_ReturnInvalidMail_When_MailIsGreaterThan100()
        {
            this.Given(x => x.GivenUserWithMail(Fixture.CreateWithLength(101)))
                .When(x => x.WhenCreateUserIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidMail))
                .BDDfy();
        }
        
        [Fact]
        public void CreateUser_Should_ReturnInvalidMail_When_MailIsInvalid()
        {
            this.Given(x => x.GivenUserWithMail(Fixture.Create<string>()))
                .When(x => x.WhenCreateUserIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidMail))
                .BDDfy();
        }
        
        [Fact]
        public void CreateUser_Should_MailAlreadyExist_When_MailAlreadyExist()
        {
            this.Given(x => x.GivenACreatedUser())
                .And(x=> x.GivenUserWithMail(_user.Mail))
                .When(x => x.WhenCreateUserIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.MailAlreadyExist))
                .BDDfy();
        }
        
        [Fact]
        public void CreateUser_Should__When_MailAlreadyExist()
        {
            this.Given(x => x.GivenUserWithMail(Fixture.CreateWithLength(21)))
                .When(x => x.WhenCreateUserIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidPassword))
                .BDDfy();
        }

        [Theory]
        [InlineData("")]
        public void CreateUser_Should_ReturnMissingPassword_When_PasswordIsMissing(string name)
        {
            this.Given(x => x.GivenUserWithPassword(name))
                .When(x => x.WhenCreateUserIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.MissingPassword))
                .BDDfy();
        }
        
        [Fact]
        public void CreateUser_Should_ReturnInvalidPassword_When_PasswordIsLessThan3()
        {
            this.Given(x => x.GivenUserWithPassword(Fixture.CreateWithLength(2)))
                .When(x => x.WhenCreateUserIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidPassword))
                .BDDfy();
        }
        
        [Fact]
        public void CreateUser_Should_ReturnOk()
        {
            this.Given(x => x.GivenAnValidUser())
                .When(x => x.WhenCreateUserIsRequested())
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
        }
        
        private void GivenUserWithMail(string mail)
        {
            _request = Fixture.Build<CreateUserRequest>()
                .With(x => x.Mail, mail)
                .Create();
        }
        
        private void GivenACreatedUser()
        {
            var request = Fixture.Build<CreateUserRequest>()
                .With(x => x.Mail, $"{Fixture.Create<string>()}@example.org")
                .Create();
            
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            var replay = client.CreateUser(request);

            replay.Should().NotBeNull();
            replay.IsSuccess.Should().BeTrue();

            _user = replay.Value;
        }
        
        private void GivenUserWithPassword(string password)
        {
            _request = Fixture.Build<CreateUserRequest>()
                .With(x => x.Mail, $"{Fixture.Create<string>()}@example.org")
                .With(x => x.Password, password)
                .Create();
        }
        
        
        private void GivenAnValidUser()
        {
            _request = Fixture.Build<CreateUserRequest>()
                .With(x => x.Mail, $"{Fixture.Create<string>()}@example.org")
                .Create();
        }
        
        private void WhenCreateUserIsRequested()
        {
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            _replay = client.CreateUser(_request);
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
            
            _replay.Value.Mail.Should().NotBeNull();
            _replay.Value.Mail.Should().Be(_request.Mail);
            
            _replay.Value.Password.Should().BeNullOrEmpty();
            
            _replay.Value.IsEnable.Should().Be(_request.IsEnable);
        }
    }
}