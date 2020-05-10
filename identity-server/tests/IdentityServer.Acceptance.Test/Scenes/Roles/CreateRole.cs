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
    public class CreateRole : BaseScene
    {
        private Role _role;
        private CreateRoleRequest _request;
        private CreateRoleReplay _replay;
        
        [Theory]
        [InlineData("")]
        public void NotCreateRoleWhenNameIsMissing(string name)
        {
            this.Given(x => x.GivenRoleWithName(name))
                .When(x => x.WhenCreateRoleIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.MissingName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateRoleWhenNameIsInvalid()
        {
            this.Given(x => x.GivenRoleWithName(Fixture.CreateWithLength(21)))
                .When(x => x.WhenCreateRoleIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateRoleWhenDisplayIsMissing()
        {
            this.Given(x => x.GivenRoleWithName(Fixture.CreateWithLength(21)))
                .When(x => x.WhenCreateRoleIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidName))
                .BDDfy();
        }

        [Fact]
        public void NotCreateRoleWhenNameAlreadyExist()
        {
            this.Given(x => x.GivenACreatedRole())
                .And(x=> x.GivenRoleWithName(_role.Name))
                .When(x => x.WhenCreateRoleIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.NameAlreadyExist))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateRoleWhenDisplayIsInvalid()
        {
            this.Given(x => x.GivenUserWithDisplayName(Fixture.CreateWithLength(51)))
                .When(x => x.WhenCreateRoleIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidDisplayName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateRoleWhenDescriptionIsInvalid()
        {
            this.Given(x => x.GivenUserWithDescription(Fixture.CreateWithLength(251)))
                .When(x => x.WhenCreateRoleIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidDescription))
                .BDDfy();
        }
        
        [Fact]
        public void CreateRoleWhenEverythingIsFine()
        {
            this.Given(x => x.GivenAnValidRole())
                .When(x => x.WhenCreateRoleIsRequested())
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
        }
        
        private void GivenRoleWithName(string name)
        {
            _request = Fixture.Build<CreateRoleRequest>()
                .With(x => x.Name, name)
                .Create();
        }
        
        private void GivenACreatedRole()
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
        
        private void GivenUserWithDisplayName(string displayName)
        {
            _request = Fixture.Build<CreateRoleRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.DisplayName, displayName)
                .Create();
        }
        
        private void GivenUserWithDescription(string description)
        {
            _request = Fixture.Build<CreateRoleRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.Description, description)
                .Create();
        }
        
        private void GivenAnValidRole()
        {
            _request = Fixture.Build<CreateRoleRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
        }
        
        private void WhenCreateRoleIsRequested()
        {
            var client = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            _replay = client.CreateRole(_request);
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
            
            _replay.Value.Name.Should().NotBeNull();
            _replay.Value.Name.Should().Be(_request.Name);
            
            _replay.Value.DisplayName.Should().NotBeNull();
            _replay.Value.DisplayName.Should().Be(_request.DisplayName);
            
            _replay.Value.Description.Should().NotBeNull();
            _replay.Value.Description.Should().Be(_request.Description);
        }
    }
}