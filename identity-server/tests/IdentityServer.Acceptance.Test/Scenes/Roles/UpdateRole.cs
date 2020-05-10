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
    public class UpdateRole : BaseScene
    {
        private Role _role;
        private UpdateRoleRequest _request;
        private UpdateRoleReplay _replay;
        
        [Theory]
        [InlineData("")]
        public void NotCreateRoleWhenNameIsMissing(string name)
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestUpdateWithName(name))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.MissingName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateRoleWhenNameIsInvalid()
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestUpdateWithName(Fixture.CreateWithLength(21)))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateRoleWhenNameAlreadyExist()
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestUpdateWithName(_role.Name))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.NameAlreadyExist));
        }

        [Theory]
        [InlineData("")]
        public void NotCreateRoleWhenDisplayIsMissing(string displayName)
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestUpdateWithDisplayName(displayName))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.MissingDisplayName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateRoleWhenDisplayIsInvalid()
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestUpdateWithDisplayName(Fixture.CreateWithLength(51)))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidDisplayName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreateRoleWhenDescriptionIsInvalid()
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestUpdateWithDescription(Fixture.CreateWithLength(251)))
                .Then(x => x.ThenIShouldGetError(DomainError.RoleError.InvalidDescription))
                .BDDfy();
        }
        
        [Fact]
        public void CreateRoleWhenEverythingIsFine()
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestUpdateWithValidRole())
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
        }
        
        [Fact]
        public void CreateRoleWhenEverythingIsFineAndNotChangeName()
        {
            this.Given(x => x.GivenARole())
                .When(x => x.WhenIRequestUpdateWithValidRoleAndNotChangeName())
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

        private void WhenIRequestUpdateWithName(string name)
        {
            _request = Fixture.Build<UpdateRoleRequest>()
                .With(x => x.Id, _role.Id)
                .With(x => x.Name, name)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithDisplayName(string displayName)
        {
            _request = Fixture.Build<UpdateRoleRequest>()
                .With(x => x.Id, _role.Id)
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.DisplayName, displayName)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithDescription(string description)
        {
            _request = Fixture.Build<UpdateRoleRequest>()
                .With(x => x.Id, _role.Id)
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.Description, description)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithValidRole()
        {
            _request = Fixture.Build<UpdateRoleRequest>()
                .With(x => x.Id, _role.Id)
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithValidRoleAndNotChangeName()
        {
            _request = Fixture.Build<UpdateRoleRequest>()
                .With(x => x.Id, _role.Id)
                .With(x => x.Name, _role.Name)
                .Create();
            
            ExecuteRequest();
        }
        
        private void ExecuteRequest()
        {
            var client = Provider.GetRequiredService<Web.Proto.Roles.RolesClient>();
            _replay = client.UpdateRole(_request);
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