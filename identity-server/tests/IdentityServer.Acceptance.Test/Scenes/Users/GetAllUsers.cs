using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Grpc.Core;
using IdentityServer.Acceptance.Test.Extensions;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using TestStack.BDDfy;
using Xunit;

namespace IdentityServer.Acceptance.Test.Scenes.Users
{
    public class GetAllUsers : BaseScene
    {
        private List<User> _users;
        private AsyncServerStreamingCall<User> _replay;
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void GetUsers_Should_ReturnOk(int length)
        {
            this.Given(x => x.GivenAUser(length))
                .When(x => x.WhenIRequestGetAllUser())
                .Then(x => x.ThenIShouldGetAllCreatedUser())
                .BDDfy();
        }
        
        private void GivenAUser(int length)
        {
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            _users = new List<User>(length);

            for (var i = 0; i < length; i++)
            {
                var request = Fixture.Build<CreateUserRequest>()
                    .With(x => x.Mail, $"{Fixture.Create<string>()}@gmail.com")
                    .Create();

                var replay = client.CreateUser(request);
                replay.Should().NotBeNull();
                replay.IsSuccess.Should().BeTrue();
                
                _users.Add(replay.Value);
            }
        }
        
        private void WhenIRequestGetAllUser()
        {
            var client = Provider.GetRequiredService<Web.Proto.Users.UsersClient>();
            _replay = client.GetUsers(new GetUsersRequest());
            _replay.Should().NotBeNull();
        }

        private async Task ThenIShouldGetAllCreatedUser()
        {
            var stream = _replay.ResponseStream;
            
            await foreach (var User in stream.ReadAllAsync())
            {
                var compared = _users.FirstOrDefault(x => x.Id.Equals(User.Id, StringComparison.InvariantCultureIgnoreCase));

                if (compared == null)
                {
                    continue;
                }
                
                User.Id.Should().Be(compared.Id);
                User.Mail.Should().Be(compared.Mail);
                User.Password.Should().BeEmpty();
                User.IsEnable.Should().Be(compared.IsEnable);
                
                _users.Remove(compared);
            }

            _users.Should().BeEmpty();
        }
    }
}