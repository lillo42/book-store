using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain.Abstractions.User;
using IdentityServer.Domain.Test.Extensions;
using IdentityServer.Domain.User;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using static IdentityServer.Domain.DomainError.UserError;

namespace IdentityServer.Domain.Test.User
{
    public class UserAggregationRootTest
    {
        private readonly ILogger<UserAggregationRoot> _logger;
        private readonly IHashAlgorithm _hash;
        private readonly IReadOnlyUserRepository _userRepository;
        private readonly IReadOnlyPermissionRepository _permissionRepository;
        private readonly IReadOnlyRoleRepository _roleRepository;
        private readonly UserAggregationRoot _aggregation;
        private readonly UserState _state;
        private readonly Common.User _entity;
        private readonly Fixture _fixture;

        public UserAggregationRootTest()
        {
            _fixture = new Fixture();

            _entity = _fixture.Create<Common.User>();

            _state = new UserState(_entity);
            _userRepository = Substitute.For<IReadOnlyUserRepository>();
            _roleRepository = Substitute.For<IReadOnlyRoleRepository>();
            _permissionRepository = Substitute.For<IReadOnlyPermissionRepository>();
            
            _hash = Substitute.For<IHashAlgorithm>();
            _logger = Substitute.For<ILogger<UserAggregationRoot>>();
            
            _aggregation = new UserAggregationRoot(_state, _hash, _userRepository, _permissionRepository, _roleRepository, _logger);
        }

        #region Create

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Create_Should_ReturnError_When_MailIsMissing(string name)
        {
            var result = await _aggregation.CreateAsync(name, _fixture.Create<string>(),
                _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingMail);
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_MailHaveLengthGreaterThan101()
        {
            var result = await _aggregation.CreateAsync(_fixture.CreateWithLength(101), 
                _fixture.Create<string>(),
                _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidMail);
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_IsNotValidMail()
        {
            var result = await _aggregation.CreateAsync(_fixture.Create<string>(), 
                _fixture.Create<string>(),
                _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidMail);
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_MailAlreadyExist()
        {
            var mail = "test@test.com";
            
            _userRepository.ExistAsync(mail, Arg.Any<CancellationToken>())
                .Returns(true);
            
            var result = await _aggregation.CreateAsync(mail, 
                _fixture.Create<string>(),
                _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MailAlreadyExist);

            var _ = _userRepository
                .Received(1)
                .ExistAsync(mail, Arg.Any<CancellationToken>());
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Create_Should_ReturnError_When_PasswordMissing(string password)
        {
            var mail = "test@test.com";
            var result = await _aggregation.CreateAsync(mail, 
                password,
                _fixture.Create<bool>());

            _userRepository.ExistAsync(mail, Arg.Any<CancellationToken>())
                .Returns(false);
            
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingPassword);
            
            var _ = _userRepository
                .Received(1)
                .ExistAsync(mail, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_DisplayHaveLengthLessThan3()
        {
            var mail = "test@test.com";
            
            _userRepository.ExistAsync(mail, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.CreateAsync("test@test.com", 
                _fixture.CreateWithLength(2),
                _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidPassword);
            
            var _ = _userRepository
                .Received(1)
                .ExistAsync(mail, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Create_Should_ReturnOk()
        {
            var mail = "test@test.com";
            var password = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();

            var hashedPassword = _fixture.Create<string>();
            
            _hash.ComputeHash(password)
                .Returns(hashedPassword);
            
            _userRepository.ExistAsync(mail, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.CreateAsync(mail, password, isEnable);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Mail.Should().Be(mail);
            _state.Password.Should().Be(hashedPassword);
            _state.IsEnable.Should().Be(isEnable);

            _hash
                .Received(1)
                .ComputeHash(password);
            
            var _ = _userRepository
                .Received(1)
                .ExistAsync(mail, Arg.Any<CancellationToken>());
        }

        #endregion

        #region Update
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Update_Should_ReturnError_When_MailIsMissing(string mail)
        {
            var result = await _aggregation.UpdateAsync(mail, _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingMail);
        }
        
        [Fact]
        public async Task Update_Should_ReturnError_When_MailHaveLengthGreaterThan101()
        {
            var result = await _aggregation.UpdateAsync(_fixture.CreateWithLength(101), _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidMail);
        }
        
        [Fact]
        public async Task Update_Should_ReturnError_When_MailIsInvalid()
        {
            var result = await _aggregation.UpdateAsync(_fixture.Create<string>(), _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidMail);
        }
        
        [Fact]
        public async Task Update_Should_ReturnError_When_MailAlreadyExist()
        {
            var mail = "test@test.com";
            
            _userRepository.ExistAsync(mail, Arg.Any<CancellationToken>())
                .Returns(true);
            
            var result = await _aggregation.UpdateAsync(mail, _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MailAlreadyExist);

            var _ = _userRepository
                .Received(1)
                .ExistAsync(mail, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Update_Should_ReturnOk()
        {
            var mail = "test@test.com";
            var isEnable = _fixture.Create<bool>();
            
            _userRepository.ExistAsync(mail, Arg.Any<CancellationToken>())
                .Returns(false);

            var result = await _aggregation.UpdateAsync(mail, isEnable);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Mail.Should().Be(mail);
            _state.IsEnable.Should().Be(isEnable);
            
            var _ = _userRepository
                .Received(1)
                .ExistAsync(mail, Arg.Any<CancellationToken>());
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
        public async Task AddPermission_Should_ReturnPermissionAlreadyExist_When_UserAlreadyHaveThePermission()
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
        
        #region Add Role
        
        [Fact]
        public async Task AddRole_Should_ReturnInvalidPermission_When_RoleIsNull()
        {
            var result = await _aggregation.AddRoleAsync(null);
            result.Should().NotBeNull();
            result.Should().Be(InvalidRole);
        }
        
        [Fact]
        public async Task AddRole_Should_ReturnInvalidPermission_When_RoleNotExist()
        {
            var role = _fixture.Create<Common.Role>();
            
            _roleRepository.ExistAsync(role.Id, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.AddRoleAsync(role);
            result.Should().NotBeNull();
            result.Should().Be(InvalidRole);

            var _ = _roleRepository
                .Received(1)
                .ExistAsync(role.Id, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task AddRole_Should_ReturnPermissionAlreadyExist_When_UserAlreadyHaveTheRole()
        {
            var role = _fixture.Create<Common.Role>();
            
            _roleRepository.ExistAsync(role.Id, Arg.Any<CancellationToken>())
                .Returns(true);

            _entity.Roles.Add(role);
            
            var result = await _aggregation.AddRoleAsync(role);
            result.Should().NotBeNull();
            result.Should().Be(RoleAlreadyExist);

            var _ = _roleRepository
                .Received(1)
                .ExistAsync(role.Id, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task AddRole()
        {
            var role = _fixture.Create<Common.Role>();
            
            _roleRepository.ExistAsync(role.Id, Arg.Any<CancellationToken>())
                .Returns(true);

            var result =  await _aggregation.AddRoleAsync(role);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Roles.Should().Contain(role);

            var _ = _roleRepository
                .Received(1)
                .ExistAsync(role.Id, Arg.Any<CancellationToken>());
        }
        #endregion
        
        #region Remove Role

        [Fact] public void RemoveRole_Should_ReturnInvalidPermission_When_RoleIsNull()
        {
            var result = _aggregation.RemoveRole(null);
            result.Should().NotBeNull();
            result.Should().Be(InvalidRole);
        }
        
        [Fact]
        public void RemoveRole_Should_ReturnNotContainsPermission_When_RoleDoesNotContains()
        {
            var role = _fixture.Create<Common.Role>();
            
            var result = _aggregation.RemoveRole(role);
            result.Should().NotBeNull();
            result.Should().Be(NotContainsRole);
        }
        
        [Fact]
        public void RemoveRole()
        {
            var role = _fixture.Create<Common.Role>();
            
            _entity.Roles.Add(role);

            var result =  _aggregation.RemoveRole(role);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Roles.Should().NotContain(role); ;
        }
        #endregion
    }
}
