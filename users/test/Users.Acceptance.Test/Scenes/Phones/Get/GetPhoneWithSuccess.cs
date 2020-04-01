using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Extensions;
using Google.Protobuf.WellKnownTypes;
using TestStack.BDDfy;
using Users.Web.Proto;
using Xunit;

namespace Users.Acceptance.Test.Scenes.Phones.Get
{
    [Story(
        IWant = "Add phone to user"
    )]
    public class GetPhoneWithSuccess : BaseScene
    {
        private GetPhoneRequest _request;
        private GetPhoneReplay _replay;
        private string _userId;
        private string[] _numbers;

        [Given(StepTitle = "Given an user")]
        private async Task GivenAnUser()
        {
            var addUserRequest = Fixture.Build<AddUserRequest>()
                
                .With(x => x.BirthDate, Timestamp.FromDateTime(Fixture.Create<DateTime>().AsUtc()))
                .With(x => x.FirstName, Fixture.Create<string>().Substring(0, 20))
                .With(x => x.Email, $"{Fixture.Create<string>()}@example.com")
                .Create();
            
            var replay = await Client.AddUsersAsync(addUserRequest);
            replay.IsSuccess.Should().BeTrue();
            _userId = replay.Value.Id;
        }
        
        [AndGiven(StepTitle =  "With phone")]
        private async Task WithPhone()
        {
            for (var i = 0; i < _numbers.Length; i++)
            {
                var request = Fixture.Build<AddPhoneRequest>()
                    .With(x => x.UserId, _userId)
                    .With(x => x.Number, Fixture.Create<string>().Substring(0, 15))
                    .Create();
            
                var replay = await Client.AddPhoneAsync(request);
                replay.IsSuccess.Should().BeTrue();
                _numbers[i] = replay.Value.Number;
            }
        }

        [When(StepTitle = "When I get user number")]
        private async Task WhenIUpdateUserInfo()
        {
            _request = Fixture.Build<GetPhoneRequest>()
                .With(x => x.UserId, _userId)
                .Create();
            
            _replay = await Client.GetPhonesAsync(_request);
        }

        [Then(StepTitle = "Then I should get all phone added ")]
        private void ThenIShouldCreateAUser()
        {
            _replay.IsSuccess.Should().BeTrue();
            _replay.ErrorCode.Should().BeNullOrEmpty();
            _replay.Description.Should().BeNullOrEmpty();

            _replay.Value.Should().HaveCount(_numbers.Length);
            _replay.Value.Should().BeEquivalentTo(_numbers.Select(x => new Phone
            {
                Number = x
            }));
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(0)]
        public void Execute(int length)
        {
            _numbers = new string[length];
            this.BDDfy();
        }
    }
}