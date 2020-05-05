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
    public class CreatePermission : BaseScene
    {
        private Permission _permission;
        private CreatePermissionRequest _request;
        private CreatePermissionReplay _replay;
        
        [Theory]
        [InlineData("")]
        public void NotCreatePermissionWhenNameIsMissing(string name)
        {
            this.Given(x => x.GivenPermissionWithName(name))
                .When(x => x.WhenCreatePermissionIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.MissingName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreatePermissionWhenNameIsInvalid()
        {
            this.Given(x => x.GivenPermissionWithName(Fixture.CreateWithLength(21)))
                .When(x => x.WhenCreatePermissionIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.InvalidName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreatePermissionWhenDisplayIsMissing()
        {
            this.Given(x => x.GivenPermissionWithName(Fixture.CreateWithLength(21)))
                .When(x => x.WhenCreatePermissionIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.InvalidName))
                .BDDfy();
        }

        [Fact]
        public void NotCreatePermissionWhenNameAlreadyExist()
        {
            this.Given(x => x.GivenACreatedPermission())
                    .And(x=> x.GivenPermissionWithName(_permission.Name))
                .When(x => x.WhenCreatePermissionIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.NameAlreadyExist))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreatePermissionWhenDisplayIsInvalid()
        {
            this.Given(x => x.GivenUserWithDisplayName(Fixture.CreateWithLength(51)))
                .When(x => x.WhenCreatePermissionIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.InvalidDisplayName))
                .BDDfy();
        }
        
        [Fact]
        public void NotCreatePermissionWhenDescriptionIsInvalid()
        {
            this.Given(x => x.GivenUserWithDescription(Fixture.CreateWithLength(251)))
                .When(x => x.WhenCreatePermissionIsRequested())
                .Then(x => x.ThenIShouldGetError(DomainError.PermissionError.InvalidDescription))
                .BDDfy();
        }
        
        [Fact]
        public void CreatePermissionWhenEverythingIsFine()
        {
            this.Given(x => x.GivenAnValidPermission())
                .When(x => x.WhenCreatePermissionIsRequested())
                .Then(x => x.ThenIShouldGetOk())
                .BDDfy();
        }
        
        private void GivenPermissionWithName(string name)
        {
            _request = Fixture.Build<CreatePermissionRequest>()
                .With(x => x.Name, name)
                .Create();
        }
        
        private void GivenACreatedPermission()
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
        
        private void GivenUserWithDisplayName(string displayName)
        {
            _request = Fixture.Build<CreatePermissionRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.DisplayName, displayName)
                .Create();
        }
        
        private void GivenUserWithDescription(string description)
        {
            _request = Fixture.Build<CreatePermissionRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .With(x => x.Description, description)
                .Create();
        }
        
        private void GivenAnValidPermission()
        {
            _request = Fixture.Build<CreatePermissionRequest>()
                .With(x => x.Name, Fixture.CreateWithLength(20))
                .Create();
        }
        
        private void WhenCreatePermissionIsRequested()
        {
            var client = Provider.GetRequiredService<Web.Proto.Permissions.PermissionsClient>();
            _replay = client.CreatePermission(_request);
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