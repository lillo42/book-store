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
    public class UpdateUser : BaseScene
    {
        private User _user;
        private UpdateUserRequest _request;
        private UpdateUserReplay _replay;
        
        [Theory]
        [InlineData("")]
        public void UpdateUser_Should_ReturnMissingMail_When_MailIsMissing(string name)
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestUpdateWithMail(name))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.MissingMail))
                .BDDfy();
        }
        
        [Fact]
        public void UpdateUser_Should_ReturnInvalidMail_When_MailIsInvalid()
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestUpdateWithMail(Fixture.Create<string>()))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidMail))
                .BDDfy();
        }
        
        [Fact]
        public void UpdateUser_Should_ReturnInvalidMail_When_MailIsGreaterThan101()
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestUpdateWithMail(Fixture.CreateWithLength(101)))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.InvalidMail))
                .BDDfy();
        }
        
        [Fact]
        public void UpdateUser_Should_ReturnMailAlreadyExist_When_MailAlreadyExist()
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestUpdateWithMail(_user.Mail))
                .Then(x => x.ThenIShouldGetError(DomainError.UserError.MailAlreadyExist));
        }

        [Fact]
        public void UpdateUser_Should_ReturnOk()
        {
            this.Given(x => x.GivenAUser())
                .When(x => x.WhenIRequestUpdateWithValidUserAndNotChangeName())
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
        }
        
        private void GivenAUser()
        {
            var request = Fixture.Build<CreateUserRequest>()
                .With(x => x.Mail, $"{Fixture.Create<string>()}@icloud.com")
                .Create();
            
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            var replay = client.CreateUser(request);

            replay.Should().NotBeNull();
            replay.IsSuccess.Should().BeTrue();
            _user = replay.Value;
        }

        private void WhenIRequestUpdateWithMail(string mail)
        {
            _request = Fixture.Build<UpdateUserRequest>()
                .With(x => x.Id, _user.Id)
                .With(x => x.Mail, mail)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithValidUserAndNotChangeName()
        {
            _request = Fixture.Build<UpdateUserRequest>()
                .With(x => x.Id, _user.Id)
                .With(x => x.Mail, _user.Mail)
                .Create();
            
            ExecuteRequest();
        }
        
        private void ExecuteRequest()
        {
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            _replay = client.UpdateUser(_request);
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
            
            _replay.Value.Password.Should().BeEmpty();
            
            _replay.Value.IsEnable.Should().Be(_request.IsEnable);
        }
    }
}