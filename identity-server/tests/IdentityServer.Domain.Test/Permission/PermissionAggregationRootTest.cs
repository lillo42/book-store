using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain.Abstractions.Permission;
using IdentityServer.Domain.Permission;
using IdentityServer.Domain.Test.Extensions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using static IdentityServer.Domain.DomainError.PermissionError;

namespace IdentityServer.Domain.Test.Permission
{
    public class PermissionAggregationRootTest
    {
        private readonly ILogger<PermissionAggregationRoot> _logger;
        private readonly IReadOnlyPermissionRepository _repository;
        private readonly PermissionAggregationRoot _aggregation;
        private readonly PermissionState _state;
        private readonly Common.Permission _entity;
        private readonly Fixture _fixture;

        public PermissionAggregationRootTest()
        {
            _fixture = new Fixture();

            _entity = _fixture.Create<Common.Permission>();

            _state = new PermissionState(_entity);
            _repository = Substitute.For<IReadOnlyPermissionRepository>();
            _logger = Substitute.For<ILogger<PermissionAggregationRoot>>();
            _aggregation = new PermissionAggregationRoot(_state, _repository, _logger);
        }
        
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
            
            _repository.ExistAsync(name, Arg.Any<CancellationToken>())
                .Returns(true);
            
            var result = await _aggregation.CreateAsync(name, 
                _fixture.Create<string>(),
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(NameAlreadyExist);

            var _ = _repository
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
            
            _repository.ExistAsync(name, Arg.Any<CancellationToken>())
                .Returns(true);
            
            var result = await _aggregation.UpdateAsync(name, 
                _fixture.Create<string>(),
                _fixture.Create<string>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(NameAlreadyExist);

            var _ = _repository
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
    }
}
