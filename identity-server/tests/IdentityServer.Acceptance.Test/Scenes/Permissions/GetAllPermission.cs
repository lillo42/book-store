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

namespace IdentityServer.Acceptance.Test.Scenes.Permissions
{
    public class GetAllPermission : BaseScene
    {
        private List<Permission> _permissions;
        private AsyncServerStreamingCall<Permission> _replay;
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void CreatePermissionWhenEverythingIsFine(int length)
        {
            this.Given(x => x.GivenAPermission(length))
                .When(x => x.WhenIRequestGetAllPermission())
                .Then(x => x.ThenIShouldGetAllCreatedPermission())
                .BDDfy();
        }
        
        private void GivenAPermission(int length)
        {
            var client = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            _permissions = new List<Permission>(length);

            for (var i = 0; i < length; i++)
            {
                var request = Fixture.Build<CreatePermissionRequest>()
                    .With(x => x.Name, Fixture.CreateWithLength(20))
                    .Create();
            
            
                var replay = client.CreatePermission(request);
                replay.Should().NotBeNull();
                replay.IsSuccess.Should().BeTrue();
                
                _permissions.Add(replay.Value);
            }
        }
        
        private void WhenIRequestGetAllPermission()
        {
            var client = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            _replay = client.GetPermissions(new GetPermissionsRequest());
            _replay.Should().NotBeNull();
        }

        private async Task ThenIShouldGetAllCreatedPermission()
        {
            var stream = _replay.ResponseStream;
            
            await foreach (var permission in stream.ReadAllAsync())
            {
                var compared = _permissions.FirstOrDefault(x => x.Id.Equals(permission.Id, StringComparison.InvariantCultureIgnoreCase));

                if (compared == null)
                {
                    continue;
                }
                
                permission.Id.Should().Be(compared.Id);
                permission.Name.Should().Be(compared.Name);
                permission.DisplayName.Should().Be(compared.DisplayName);
                permission.Description.Should().Be(compared.Description);
                
                _permissions.Remove(compared);
            }

            _permissions.Should().BeEmpty();
        }
    }
}