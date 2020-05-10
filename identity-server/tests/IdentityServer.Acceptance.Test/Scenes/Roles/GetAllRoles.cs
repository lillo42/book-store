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

namespace IdentityServer.Acceptance.Test.Scenes.Roles
{
    public class GetAllRoles : BaseScene
    {
        private List<Role> _roles;
        private AsyncServerStreamingCall<Role> _replay;
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void CreateRoleWhenEverythingIsFine(int length)
        {
            this.Given(x => x.GivenARole(length))
                .When(x => x.WhenIRequestGetAllRole())
                .Then(x => x.ThenIShouldGetAllCreatedRole())
                .BDDfy();
        }
        
        private void GivenARole(int length)
        {
            var client = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            _roles = new List<Role>(length);

            for (var i = 0; i < length; i++)
            {
                var request = Fixture.Build<CreateRoleRequest>()
                    .With(x => x.Name, Fixture.CreateWithLength(20))
                    .Create();

                var replay = client.CreateRole(request);
                replay.Should().NotBeNull();
                replay.IsSuccess.Should().BeTrue();
                
                _roles.Add(replay.Value);
            }
        }
        
        private void WhenIRequestGetAllRole()
        {
            var client = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            _replay = client.GetRoles(new GetRolesRequest());
            _replay.Should().NotBeNull();
        }

        private async Task ThenIShouldGetAllCreatedRole()
        {
            var stream = _replay.ResponseStream;
            
            await foreach (var role in stream.ReadAllAsync())
            {
                var compared = _roles.FirstOrDefault(x => x.Id.Equals(role.Id, StringComparison.InvariantCultureIgnoreCase));

                if (compared == null)
                {
                    continue;
                }
                
                role.Id.Should().Be(compared.Id);
                role.Name.Should().Be(compared.Name);
                role.DisplayName.Should().Be(compared.DisplayName);
                role.Description.Should().Be(compared.Description);
                
                _roles.Remove(compared);
            }

            _roles.Should().BeEmpty();
        }
    }
}