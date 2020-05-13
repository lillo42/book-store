using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain.Abstractions.Client;
using IdentityServer.Domain.Client;
using IdentityServer.Domain.Test.Extensions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using static IdentityServer.Domain.DomainError.ClientError;

namespace IdentityServer.Domain.Test.Client
{
    public class ClientAggregationRootTest
    {
        private readonly ILogger<ClientAggregationRoot> _logger;
        private readonly IReadOnlyClientRepository _clientRepository;
        private readonly IReadOnlyResourceRepository _resourceRepository;
        private readonly IReadOnlyPermissionRepository _permissionRepository;
        private readonly IReadOnlyRoleRepository _roleRepository;
        private readonly ClientAggregationRoot _aggregation;
        private readonly ClientState _state;
        private readonly Common.Client _entity;
        private readonly Fixture _fixture;

        public ClientAggregationRootTest()
        {
            _fixture = new Fixture();

            _entity = _fixture.Create<Common.Client>();

            _state = new ClientState(_entity);
            _clientRepository = Substitute.For<IReadOnlyClientRepository>();
            _roleRepository = Substitute.For<IReadOnlyRoleRepository>();
            _permissionRepository = Substitute.For<IReadOnlyPermissionRepository>();
            _resourceRepository = Substitute.For<IReadOnlyResourceRepository>();
            
            _logger = Substitute.For<ILogger<ClientAggregationRoot>>();
            
            _aggregation = new ClientAggregationRoot(_state, 
                _clientRepository,
                _permissionRepository,
                _roleRepository,
                _resourceRepository,
                _logger);
        }

        #region Create

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Create_Should_ReturnMissingName_When_NameIsMissing(string name)
        {
            var result = await _aggregation.CreateAsync(name, _fixture.Create<string>(), 
                _fixture.Create<string>(), _fixture.Create<bool>());
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingName);
        }
        
        [Fact]
        public async Task Create_Should_ReturnInvalidName_When_NameHaveLengthGreaterThan101()
        {
            var result = await _aggregation.CreateAsync(_fixture.CreateWithLength(101), _fixture.Create<string>(),
                _fixture.Create<string>(), _fixture.Create<bool>());
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidName);
        }
        
        [Fact]
        public async Task Create_Should_ReturnNameAlreadyExist_When_NameAlreadyExist()
        {
            var name = _fixture.Create<string>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(true);
            
            var result = await _aggregation.CreateAsync(name, _fixture.Create<string>(), 
                _fixture.Create<string>(), _fixture.Create<bool>());
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(NameAlreadyExist);
        
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Create_Should_ReturnMissingClientId_When_ClientIdIsMissing(string clientId)
        {
            var name = _fixture.Create<string>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.CreateAsync(name, clientId,
                _fixture.Create<string>(), _fixture.Create<bool>());
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingClientId);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Create_Should_ReturnClientId_When_ClientHaveLengthGreaterThan50()
        {
            var name = _fixture.Create<string>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.CreateAsync(name, _fixture.CreateWithLength(51),
                _fixture.Create<string>(), _fixture.Create<bool>());
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidClientId);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Create_Should_ReturnClientIdAlreadyExist_When_ClientIdAlreadyExist()
        {
            var name = _fixture.Create<string>();
            var clientId = _fixture.Create<string>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            _clientRepository.ExistClientIdAsync(clientId, Arg.Any<CancellationToken>())
                .Returns(true);
            
            var result = await _aggregation.CreateAsync(name, clientId, 
                _fixture.Create<string>(), _fixture.Create<bool>());
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(ClientIdAlreadyExist);
        
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
            
            var __ = _clientRepository
                .Received(1)
                .ExistClientIdAsync(clientId, Arg.Any<CancellationToken>());
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Create_Should_ReturnMissingClientSecret_When_ClientSecretIsMissing(string clientSecret)
        {
            var name = _fixture.Create<string>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            
            var clientId = _fixture.Create<string>();
            _clientRepository.ExistClientIdAsync(clientId, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.CreateAsync(name, clientId,
                clientSecret, _fixture.Create<bool>());
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingClientSecret);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
            
            var __ = _clientRepository
                .Received(1)
                .ExistClientIdAsync(clientId, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Create_Should_ReturnClientSecret_When_ClientSecreteHaveLengthGreaterThan250()
        {
            var name = _fixture.Create<string>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var clientId = _fixture.Create<string>();
            _clientRepository.ExistClientIdAsync(clientId, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.CreateAsync(name, clientId,  
                _fixture.CreateWithLength(251), _fixture.Create<bool>());
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidClientSecret);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
            
            var __ = _clientRepository
                .Received(1)
                .ExistClientIdAsync(clientId, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Create_Should_ReturnOk()
        {
            var name = _fixture.Create<string>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var clientId = _fixture.Create<string>();
            _clientRepository.ExistClientIdAsync(clientId, Arg.Any<CancellationToken>())
                .Returns(false);

            var clientSecret = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            var result = await _aggregation.CreateAsync(name, clientId, clientSecret, isEnable);
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Name.Should().Be(name);
            _state.ClientId.Should().Be(clientId);
            _state.ClientSecret.Should().Be(clientSecret);
            _state.IsEnable.Should().Be(isEnable);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
            
            var __ = _clientRepository
                .Received(1)
                .ExistClientIdAsync(clientId, Arg.Any<CancellationToken>());
        }
        
        #endregion

        #region Update
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Update_Should_ReturnMissingName_When_NameIsMissing(string name)
        {
            var clientId = _fixture.Create<string>();
            var clientSecret = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            var result = await _aggregation.UpdateAsync(name, clientId, 
                clientSecret, isEnable);
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingName);

            _state.Name.Should().NotBe(name);
            _state.ClientId.Should().NotBe(clientId);
            _state.ClientSecret.Should().NotBe(clientSecret);
        }
        
        [Fact]
        public async Task Update_Should_ReturnInvalidName_When_NameHaveLengthGreaterThan101()
        {
            var name = _fixture.CreateWithLength(101);
            var clientId = _fixture.Create<string>();
            var clientSecret = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            var result = await _aggregation.UpdateAsync(name, clientId, 
                clientSecret, isEnable);
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidName);
            
            _state.Name.Should().NotBe(name);
            _state.ClientId.Should().NotBe(clientId);
            _state.ClientSecret.Should().NotBe(clientSecret);
        }
        
        [Fact]
        public async Task Update_Should_ReturnNameAlreadyExist_When_NameAlreadyExist()
        {
            var name = _fixture.Create<string>();
            var clientId = _fixture.Create<string>();
            var clientSecret = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(true);
            
            var result = await _aggregation.UpdateAsync(name, clientId, 
                clientSecret, isEnable);
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(NameAlreadyExist);
        
            _state.Name.Should().NotBe(name);
            _state.ClientId.Should().NotBe(clientId);
            _state.ClientSecret.Should().NotBe(clientSecret);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Update_Should_ReturnMissingClientId_When_ClientIdIsMissing(string clientId)
        {
            var name = _fixture.Create<string>();
            var clientSecret = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.UpdateAsync(name, clientId, clientSecret, isEnable);
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingClientId);
            
            _state.Name.Should().NotBe(name);
            _state.ClientId.Should().NotBe(clientId);
            _state.ClientSecret.Should().NotBe(clientSecret);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Update_Should_ReturnClientId_When_ClientHaveLengthGreaterThan50()
        {
            var name = _fixture.Create<string>();
            var clientId = _fixture.CreateWithLength(51);
            var clientSecret = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.UpdateAsync(name, clientId, clientSecret, isEnable);
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidClientId);
            
            _state.Name.Should().NotBe(name);
            _state.ClientId.Should().NotBe(clientId);
            _state.ClientSecret.Should().NotBe(clientSecret);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Update_Should_ReturnClientIdAlreadyExist_When_ClientIdAlreadyExist()
        {
            var name = _fixture.Create<string>();
            var clientId = _fixture.Create<string>();
            var clientSecret = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            _clientRepository.ExistClientIdAsync(clientId, Arg.Any<CancellationToken>())
                .Returns(true);
            
            var result = await _aggregation.UpdateAsync(name, clientId, clientSecret, isEnable);
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(ClientIdAlreadyExist);
            
            _state.Name.Should().NotBe(name);
            _state.ClientId.Should().NotBe(clientId);
            _state.ClientSecret.Should().NotBe(clientSecret);
        
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
            
            var __ = _clientRepository
                .Received(1)
                .ExistClientIdAsync(clientId, Arg.Any<CancellationToken>());
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Update_Should_ReturnMissingClientSecret_When_ClientSecretIsMissing(string clientSecret)
        {
            var name = _fixture.Create<string>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            
            var clientId = _fixture.Create<string>();
            _clientRepository.ExistClientIdAsync(clientId, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.CreateAsync(name, clientId, clientSecret, _fixture.Create<bool>());
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingClientSecret);
            
            _state.Name.Should().NotBe(name);
            _state.ClientId.Should().NotBe(clientId);
            _state.ClientSecret.Should().NotBe(clientSecret);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
            
            var __ = _clientRepository
                .Received(1)
                .ExistClientIdAsync(clientId, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Update_Should_ReturnClientSecret_When_ClientSecreteHaveLengthGreaterThan250()
        {
            var name = _fixture.Create<string>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var clientId = _fixture.Create<string>();
            _clientRepository.ExistClientIdAsync(clientId, Arg.Any<CancellationToken>())
                .Returns(false);

            var clientSecret = _fixture.CreateWithLength(251);
            var result = await _aggregation.CreateAsync(name, clientId,  clientSecret, _fixture.Create<bool>());
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidClientSecret);
            
            _state.Name.Should().NotBe(name);
            _state.ClientId.Should().NotBe(clientId);
            _state.ClientSecret.Should().NotBe(clientSecret);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
            
            var __ = _clientRepository
                .Received(1)
                .ExistClientIdAsync(clientId, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Update_Should_ReturnOk()
        {
            var name = _fixture.Create<string>();
            
            _clientRepository.ExistNameAsync(name, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var clientId = _fixture.Create<string>();
            _clientRepository.ExistClientIdAsync(clientId, Arg.Any<CancellationToken>())
                .Returns(false);

            var clientSecret = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            var result = await _aggregation.UpdateAsync(name, clientId, clientSecret, isEnable);
        
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Name.Should().Be(name);
            _state.ClientId.Should().Be(clientId);
            _state.ClientSecret.Should().Be(clientSecret);
            _state.IsEnable.Should().Be(isEnable);
            
            var _ = _clientRepository
                .Received(1)
                .ExistNameAsync(name, Arg.Any<CancellationToken>());
            
            var __ = _clientRepository
                .Received(1)
                .ExistClientIdAsync(clientId, Arg.Any<CancellationToken>());
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
        
            _state.Permissions.Should().NotContain(permission);
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
        
            _state.Roles.Should().NotContain(role);
        }
        #endregion
        
        #region Add Resource
        
        [Fact]
        public async Task AddResource_Should_ReturnInvalidPermission_When_ResourceIsNull()
        {
            var result = await _aggregation.AddResourceAsync(null);
            result.Should().NotBeNull();
            result.Should().Be(InvalidResource);
        }
        
        [Fact]
        public async Task AddResource_Should_ReturnInvalidPermission_When_ResourceNotExist()
        {
            var resource = _fixture.Create<Common.Resource>();
            
            _resourceRepository.ExistAsync(resource.Id, Arg.Any<CancellationToken>())
                .Returns(false);
            
            var result = await _aggregation.AddResourceAsync(resource);
            result.Should().NotBeNull();
            result.Should().Be(InvalidResource);
        
            var _ = _resourceRepository
                .Received(1)
                .ExistAsync(resource.Id, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task AddResource_Should_ReturnResourceAlreadyExist_When_ResourceAlreadyHaveTheRole()
        {
            var resource = _fixture.Create<Common.Resource>();
            
            _resourceRepository.ExistAsync(resource.Id, Arg.Any<CancellationToken>())
                .Returns(true);
        
            _entity.Resources.Add(resource);
            
            var result = await _aggregation.AddResourceAsync(resource);
            result.Should().NotBeNull();
            result.Should().Be(ResourceAlreadyExist);
        
            var _ = _resourceRepository
                .Received(1)
                .ExistAsync(resource.Id, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task AddResource()
        {
            var resource = _fixture.Create<Common.Resource>();
            
            _resourceRepository.ExistAsync(resource.Id, Arg.Any<CancellationToken>())
                .Returns(true);
        
            var result =  await _aggregation.AddResourceAsync(resource);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        
            _state.Resources.Should().Contain(resource);
        
            var _ = _resourceRepository
                .Received(1)
                .ExistAsync(resource.Id, Arg.Any<CancellationToken>());
        }
        #endregion
        
        #region Remove Role
        
        [Fact] public void RemoveResource_Should_ReturnInvalidPermission_When_ResourceIsNull()
        {
            var result = _aggregation.RemoveResource(null);
            result.Should().NotBeNull();
            result.Should().Be(InvalidResource);
        }
        
        [Fact]
        public void RemoveResource_Should_ReturnNotContainsPermission_When_ResourceDoesNotContains()
        {
            var resource = _fixture.Create<Common.Resource>();
            
            var result = _aggregation.RemoveResource(resource);
            result.Should().NotBeNull();
            result.Should().Be(NotContainsResource);
        }
        
        [Fact]
        public void RemoveResource()
        {
            var resource = _fixture.Create<Common.Resource>();
            
            _entity.Resources.Add(resource);
        
            var result =  _aggregation.RemoveResource(resource);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        
            _state.Resources.Should().NotContain(resource);
        }
        #endregion
    }
}
