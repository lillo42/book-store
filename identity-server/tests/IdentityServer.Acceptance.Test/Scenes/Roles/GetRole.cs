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

namespace IdentityServer.Acceptance.Test.Scenes.Roles
{
    public class GetRole : BaseScene
    {
        private Role _role;
        private GetRoleByIeReplay _replay;
        
        [Theory]
        [InlineData("")]
        [InlineData("ABC")]
        public void GetRole_Should_ReturnInvalid_When_IdIsInvalid(string id)
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestGetRole(id))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidId))
                .BDDfy();
        }
        
        [Fact]
        public void GetRole_Should_ReturnNotFound_When_RoleNotFound()
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestGetRole(Fixture.Create<Guid>().ToString()))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.NotFound))
                .BDDfy();
        }
        
        [Fact]
        public void CreateRoleWhenEverythingIsFine()
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestGetRole(_role.Id))
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
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
        
        private void WhenIRequestGetRole(string id)
        {
            var client = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            _replay = client.GetRoleById(new GetRoleByIdRequest
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
            _replay.Value.Id.Should().Be(_role.Id);
            
            _replay.Value.Name.Should().NotBeNull();
            _replay.Value.Name.Should().Be(_role.Name);
            
            _replay.Value.DisplayName.Should().NotBeNull();
            _replay.Value.DisplayName.Should().Be(_role.DisplayName);
            
            _replay.Value.Description.Should().NotBeNull();
            _replay.Value.Description.Should().Be(_role.Description);
        }
    }
}