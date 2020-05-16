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

namespace IdentityServer.Acceptance.Test.Scenes.Permissions
{
    public class GetPermission : BaseScene
    {
        private Permission _permission;
        private GetPermissionByIdReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData("ABC")]
        public void GetPermission_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestGetPermission(id))
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void GetPermission_Should_ReturnNotFound_When_PermissionNotFound()
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestGetPermission(Fixture.Create<Guid>().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void CreatePermissionWhenEverythingIsFine()
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestGetPermission(_permission.Id))
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
        }
        
        private void GivenAPermission()
        {
            var request = Fixture.Build<CreatePermissionRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            var client = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            var replay = client.CreatePermission(request);

            replay.Should().NotBeNull();
            replay.IsSuccess.Should().BeTrue();
            _permission = replay.Value;
        }
        
        private void WhenIRequestGetPermission(string id)
        {
            var client = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            _replay = client.GetPermissionById(new GetPermissionByIdRequest
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
            _replay.Value.Id.Should().Be(_permission.Id);
            
            _replay.Value.Name.Should().NotBeNull();
            _replay.Value.Name.Should().Be(_permission.Name);
            
            _replay.Value.DisplayName.Should().NotBeNull();
            _replay.Value.DisplayName.Should().Be(_permission.DisplayName);
            
            _replay.Value.Description.Should().NotBeNull();
            _replay.Value.Description.Should().Be(_permission.Description);
        }
    }
}