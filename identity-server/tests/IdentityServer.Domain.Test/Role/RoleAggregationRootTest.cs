using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain.Abstractions.Role;
using IdentityServer.Domain.Role;
using IdentityServer.Domain.Test.Extensions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using static IdentityServer.Domain.DomainError.RoleError;

namespace IdentityServer.Domain.Test.Role
{
    public class RoleAggregationRootTest
    {
        private readonly ILogger<RoleAggregationRoot> _logger;
        private readonly IReadOnlyRoleRepository _roleRepository;
        private readonly IReadOnlyPermissionRepository _permissionRepository;
        private readonly RoleAggregationRoot _aggregation;
        private readonly RoleState _state;
        private readonly Common.Role _entity;
        private readonly Fixture _fixture;

        public RoleAggregationRootTest()
        {
            _fixture = new Fixture();

            _entity = _fixture.Create<Common.Role>();

            _state = new RoleState(_entity);
            _roleRepository = Substitute.For<IReadOnlyRoleRepository>();
            _permissionRepository = Substitute.For<IReadOnlyPermissionRepository>();
            _logger = Substitute.For<ILogger<RoleAggregationRoot>>();
            _aggregation = new RoleAggregationRoot(_state, _roleRepository, _permissionRepository, _logger);
        }

        #region Create

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Create_Should_ReturnError_When_NameIsMissing(string name)
        {
            var result = await _aggregation.CreateAsync(name, _fixture.Create<string>(),
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingName);
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_NameHaveLengthGreaterThan20()
        {
            var result = await _aggregation.CreateAsync(_fixture.CreateWithLength(21), 
                _fixture.Create<string>(),
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidName);
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_NameAlreadyExist()
        {
            var name = _fixture.CreateWithLength(20);
            
            _roleRepository.ExistAsync(name, Arg.Any<CancellationToken>())
                .Returns(true);
            
            var result = await _aggregation.CreateAsync(name, 
                _fixture.Create<string>(),
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(NameAlreadyExist);

            var _ = _roleRepository
                .Received(1)
                .ExistAsync(name, Arg.Any<CancellationToken>());
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Create_Should_ReturnError_When_DisplayIsMissing(string displayName)
        {
            var result = await _aggregation.CreateAsync(_fixture.CreateWithLength(20), 
                displayName,
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingDisplayName);
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_DisplayHaveLengthGreaterThan50()
        {
            var result = await _aggregation.CreateAsync(_fixture.CreateWithLength(20), 
                _fixture.CreateWithLength(51),
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidDisplayName);
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_DescriptionHaveLengthGreaterThan250()
        {
            var result = await _aggregation.CreateAsync(_fixture.CreateWithLength(20), 
                _fixture.CreateWithLength(50),
                _fixture.CreateWithLength(251));

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidDescription);
        }
        
        [Fact]
        public async Task Create_Should_ReturnOk_When_DescriptionIsNull()
        {
            var name = _fixture.CreateWithLength(20);
            var displayName = _fixture.CreateWithLength(50);
            
            var result = await _aggregation.CreateAsync(name, displayName, null);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Name.Should().Be(name);
            _state.DisplayName.Should().Be(displayName);
            _state.Description.Should().BeNullOrEmpty();
        }
        
        [Fact]
        public async Task Create_Should_ReturnOk_When_DescriptionIsNotNull()
        {
            var name = _fixture.CreateWithLength(20);
            var displayName = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            
            var result = await _aggregation.CreateAsync(name, displayName, description);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Name.Should().Be(name);
            _state.DisplayName.Should().Be(displayName);
            _state.Description.Should().Be(description);
        }

        #endregion

        #region Update
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Update_Should_ReturnError_When_NameIsMissing(string name)
        {
            var result = await _aggregation.UpdateAsync(name, _fixture.Create<string>(),
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingName);
        }
        
        [Fact]
        public async Task Update_Should_ReturnError_When_NameHaveLengthGreaterThan20()
        {
            var result = await _aggregation.UpdateAsync(_fixture.CreateWithLength(21), 
                _fixture.Create<string>(),
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidName);
        }
        
        [Fact]
        public async Task Update_Should_ReturnError_When_NameAlreadyExist()
        {
            var name = _fixture.CreateWithLength(20);
            
            _roleRepository.ExistAsync(name, Arg.Any<CancellationToken>())
                .Returns(true);
            
            var result = await _aggregation.UpdateAsync(name, 
                _fixture.Create<string>(),
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(NameAlreadyExist);

            var _ = _roleRepository
                .Received(1)
                .ExistAsync(name, Arg.Any<CancellationToken>());
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Update_Should_ReturnError_When_DisplayIsMissing(string displayName)
        {
            var result = await _aggregation.UpdateAsync(_fixture.CreateWithLength(20), 
                displayName,
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingDisplayName);
        }
        
        [Fact]
        public async Task Update_Should_ReturnError_When_DisplayHaveLengthGreaterThan50()
        {
            var result = await _aggregation.UpdateAsync(_fixture.CreateWithLength(20), 
                _fixture.CreateWithLength(51),
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidDisplayName);
        }
        
        [Fact]
        public async Task Update_Should_ReturnError_When_DescriptionHaveLengthGreaterThan250()
        {
            var result = await _aggregation.UpdateAsync(_fixture.CreateWithLength(20), 
                _fixture.CreateWithLength(50),
                _fixture.CreateWithLength(251));

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidDescription);
        }
        
        [Fact]
        public async Task Update_Should_ReturnOk_When_DescriptionIsNull()
        {
            var name = _fixture.CreateWithLength(20);
            var displayName = _fixture.CreateWithLength(50);
            
            var result = await _aggregation.UpdateAsync(name, displayName, null);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Name.Should().Be(name);
            _state.DisplayName.Should().Be(displayName);
            _state.Description.Should().BeNullOrEmpty();
        }
        
        [Fact]
        public async Task Update_Should_ReturnOk_When_DescriptionIsNotNull()
        {
            var name = _fixture.CreateWithLength(20);
            var displayName = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            
            var result = await _aggregation.UpdateAsync(name, displayName, description);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Name.Should().Be(name);
            _state.DisplayName.Should().Be(displayName);
            _state.Description.Should().Be(description);
        }
        #endregion

        #region Add Permission
        
        [Fact]
        public async Task AddPermission_Should_ReturnInvalidPermission_When_PermissionIsNull()
        {
            var result = await _aggregation.AddPermissionAsync(null);
            result.Should().NotBeNull();
            result.Should().Be(InvalidPermission);
        }
        
        [Fact]
        public async Task AddPermission_Should_ReturnInvalidPermission_When_PermissionNotExist()
        {
            var permission = _fixture.Create<Common.Permission>();
            
            _permissionRepository.ExistAsync(permission.Id, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.AddPermissionAsync(permission);
            result.Should().NotBeNull();
            result.Should().Be(InvalidPermission);

            var _ = _permissionRepository
                .Received(1)
                .ExistAsync(permission.Id, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task AddPermission_Should_ReturnPermissionAlreadyExist_When_RoleAlreadyHaveThePermission()
        {
            var permission = _fixture.Create<Common.Permission>();
            
            _permissionRepository.ExistAsync(permission.Id, Arg.Any<CancellationToken>())
                .Returns(true);

            _entity.Permissions.Add(permission);
            
            var result = await _aggregation.AddPermissionAsync(permission);
            result.Should().NotBeNull();
            result.Should().Be(PermissionAlreadyExist);

            var _ = _permissionRepository
                .Received(1)
                .ExistAsync(permission.Id, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task AddPermission()
        {
            var permission = _fixture.Create<Common.Permission>();
            
            _permissionRepository.ExistAsync(permission.Id, Arg.Any<CancellationToken>())
                .Returns(true);

            var result =  await _aggregation.AddPermissionAsync(permission);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Permissions.Should().Contain(permission);

            var _ = _permissionRepository
                .Received(1)
                .ExistAsync(permission.Id, Arg.Any<CancellationToken>());
        }
        #endregion
        
        #region Remove Permission
        
        [Fact]
        public void RemovePermission_Should_ReturnInvalidPermission_When_PermissionIsNull()
        {
            var result = _aggregation.RemovePermission(null);
            result.Should().NotBeNull();
            result.Should().Be(InvalidPermission);
        }
        
        [Fact]
        public void RemovePermission_Should_ReturnNotContainsPermission_When_RoleDoesNotContains()
        {
            var permission = _fixture.Create<Common.Permission>();
            
            var result = _aggregation.RemovePermission(permission);
            result.Should().NotBeNull();
            result.Should().Be(NotContainsPermission);
        }
        
        [Fact]
        public void RemovePermission()
        {
            var permission = _fixture.Create<Common.Permission>();
            
            _entity.Permissions.Add(permission);

            var result =  _aggregation.RemovePermission(permission);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Permissions.Should().NotContain(permission); ;
        }
        #endregion
    }
}
