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
    public class UpdatePermission : BaseScene
    {
        private Permission _permission;
        private UpdatePermissionRequest _request;
        private UpdatePermissionReplay _replay;
        
        [Theory]
        [InlineData("")]
        public void NotCreatePermissionWhenNameIsMissing(string name)
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestUpdateWithName(name))
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.MissingName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreatePermissionWhenNameIsInvalid()
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestUpdateWithName(Fixture.CreateWithLength(21)))
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.InvalidName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreatePermissionWhenNameAlreadyExist()
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestUpdateWithName(_permission.Name))
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.NameAlreadyExist));
        }

        [Theory]
        [InlineData("")]
        public void NotCreatePermissionWhenDisplayIsMissing(string displayName)
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestUpdateWithDisplayName(displayName))
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.MissingDisplayName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreatePermissionWhenDisplayIsInvalid()
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestUpdateWithDisplayName(Fixture.CreateWithLength(51)))
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.InvalidDisplayName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreatePermissionWhenDescriptionIsInvalid()
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestUpdateWithDescription(Fixture.CreateWithLength(251)))
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.InvalidDescription))
                .BDDfy();
        }
        
        [Fact]
        public void CreatePermissionWhenEverythingIsFine()
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestUpdateWithValidPermission())
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
        }
        
        [Fact]
        public void CreatePermissionWhenEverythingIsFineAndNotChangeName()
        {
            this.Given(x => x.GivenAPermission())
                .When(x => x.WhenIRequestUpdateWithValidPermissionAndNotChangeName())
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

        private void WhenIRequestUpdateWithName(string name)
        {
            _request = Fixture.Build<UpdatePermissionRequest>()
                .With(x => x.Id, _permission.Id)
                .With(x => x.Name, name)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithDisplayName(string displayName)
        {
            _request = Fixture.Build<UpdatePermissionRequest>()
                .With(x => x.Id, _permission.Id)
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.DisplayName, displayName)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithDescription(string description)
        {
            _request = Fixture.Build<UpdatePermissionRequest>()
                .With(x => x.Id, _permission.Id)
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.Description, description)
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithValidPermission()
        {
            _request = Fixture.Build<UpdatePermissionRequest>()
                .With(x => x.Id, _permission.Id)
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
            
            ExecuteRequest();
        }
        
        private void WhenIRequestUpdateWithValidPermissionAndNotChangeName()
        {
            _request = Fixture.Build<UpdatePermissionRequest>()
                .With(x => x.Id, _permission.Id)
                .With(x => x.Name, _permission.Name)
                .Create();
            
            ExecuteRequest();
        }
        
        private void ExecuteRequest()
        {
            var client = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            _replay = client.UpdatePermission(_request);
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