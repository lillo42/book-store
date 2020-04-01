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

namespace Users.Acceptance.Test.Scenes.Address.Get
{
    [Story(
        IWant = "Get user address"
    )]
    public class GetPhoneWithSuccess : BaseScene
    {
        private GetAddressesRequest _request;
        private GetAddressesReplay _replay;
        private string _userId;
        private Web.Proto.Address[] _address;

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
        
        [AndGiven(StepTitle =  "With Address")]
        private async Task WithAddress()
        {
            for (var i = 0; i < _address.Length; i++)
            {
                var request = Fixture.Build<AddAddressRequest>()
                    .With(x => x.UserId, _userId)
                    .With(x => x.Number, _address[i].Number)
                    .With(x => x.Line, _address[i].Line)
                    .With(x => x.PostCode, _address[i].PostCode)
                    .Create();
            
                var replay = await Client.AddAddressAsync(request);
                replay.IsSuccess.Should().BeTrue();
                _address[i].Id = replay.Value.Id;
            }
        }

        [When(StepTitle = "When I get user address")]
        private async Task WhenIGetUserAddress()
        {
            _request = Fixture.Build<GetAddressesRequest>()
                .With(x => x.UserId, _userId)
                .Create();
            
            _replay = await Client.GetAddressesAsync(_request);
        }

        [Then(StepTitle = "Then I should get all address added")]
        private void ThenIShouldGetAllAddressAdded()
        {
            _replay.IsSuccess.Should().BeTrue();
            _replay.ErrorCode.Should().BeNullOrEmpty();
            _replay.Description.Should().BeNullOrEmpty();

            _replay.Value.Should().HaveCount(_address.Length);
            _replay.Value.Should().BeEquivalentTo(_address);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(0)]
        public void Execute(int length)
        {
            _address = new Web.Proto.Address[length];
            for (var i = 0; i < _address.Length; i++)
            {
                _address[i] = Fixture.Build<Web.Proto.Address>()
                    .With(x => x.PostCode, Fixture.Create<string>().Substring(0, 10))
                    .With(x => x.Number, Math.Abs(Fixture.Create<int>()))
                    .Create();
            }
            this.BDDfy();
        }
    }
}