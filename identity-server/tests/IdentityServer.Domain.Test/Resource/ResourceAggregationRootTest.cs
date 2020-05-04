using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Domain.Abstractions.Resource;
using IdentityServer.Domain.Resource;
using IdentityServer.Domain.Test.Extensions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using static IdentityServer.Domain.DomainError.ResourceError;

namespace IdentityServer.Domain.Test.Resource
{
    public class ResourceAggregationRootTest
    {
        private readonly ILogger<ResourceAggregationRoot> _logger;
        private readonly IReadOnlyResourceRepository _repository;
        private readonly ResourceAggregationRoot _aggregation;
        private readonly ResourceState _state;
        private readonly Common.Resource _entity;
        private readonly Fixture _fixture;

        public ResourceAggregationRootTest()
        {
            _fixture = new Fixture();

            _entity = _fixture.Create<Common.Resource>();

            _state = new ResourceState(_entity);
            _repository = Substitute.For<IReadOnlyResourceRepository>();
            _logger = Substitute.For<ILogger<ResourceAggregationRoot>>();
            _aggregation = new ResourceAggregationRoot(_state, _repository, _logger);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Create_Should_ReturnError_When_NameIsMissing(string name)
        {
            var result = await _aggregation.CreateAsync(name, _fixture.Create<string>(),
                _fixture.Create<string>(), _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingName);
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_NameHaveLengthGreaterThan20()
        {
            var result = await _aggregation.CreateAsync(_fixture.CreateWithLength(21), 
                _fixture.Create<string>(),
                _fixture.Create<string>(), _fixture.Create<bool>());

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
                _fixture.Create<string>(), _fixture.Create<bool>());

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
                _fixture.Create<string>(), _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingDisplayName);
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_DisplayHaveLengthGreaterThan50()
        {
            var result = await _aggregation.CreateAsync(_fixture.CreateWithLength(20), 
                _fixture.CreateWithLength(51),
                _fixture.Create<string>(), _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidDisplayName);
        }
        
        [Fact]
        public async Task Create_Should_ReturnError_When_DescriptionHaveLengthGreaterThan250()
        {
            var result = await _aggregation.CreateAsync(_fixture.CreateWithLength(20), 
                _fixture.CreateWithLength(50),
                _fixture.CreateWithLength(251), _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidDescription);
        }
        
        [Fact]
        public async Task Create_Should_ReturnOk_When_DescriptionIsNull()
        {
            var name = _fixture.CreateWithLength(20);
            var displayName = _fixture.CreateWithLength(50);
            var isEnable = _fixture.Create<bool>();
            var result = await _aggregation.CreateAsync(name, 
                displayName,
                null, isEnable);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Name.Should().Be(name);
            _state.DisplayName.Should().Be(displayName);
            _state.Description.Should().BeNullOrEmpty();
            _state.IsEnable.Should().Be(isEnable);
        }
        
        [Fact]
        public async Task Create_Should_ReturnOk_When_DescriptionIsNotNull()
        {
            var name = _fixture.CreateWithLength(20);
            var displayName = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            var result = await _aggregation.CreateAsync(name, displayName,
                description, isEnable);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Name.Should().Be(name);
            _state.DisplayName.Should().Be(displayName);
            _state.Description.Should().Be(description);
            _state.IsEnable.Should().Be(isEnable);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Update_Should_ReturnError_When_NameIsMissing(string name)
        {
            var result = await _aggregation.UpdateAsync(name, _fixture.Create<string>(),
                _fixture.Create<string>(), _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingName);
        }
        
        [Fact]
        public async Task Update_Should_ReturnError_When_NameHaveLengthGreaterThan20()
        {
            var result = await _aggregation.UpdateAsync(_fixture.CreateWithLength(21), 
                _fixture.Create<string>(),
                _fixture.Create<string>(), _fixture.Create<bool>());

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
                _fixture.Create<string>(), _fixture.Create<bool>());

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
                _fixture.Create<string>(), _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(MissingDisplayName);
        }
        
        [Fact]
        public async Task Update_Should_ReturnError_When_DisplayHaveLengthGreaterThan50()
        {
            var result = await _aggregation.UpdateAsync(_fixture.CreateWithLength(20), 
                _fixture.CreateWithLength(51),
                _fixture.Create<string>(), _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidDisplayName);
        }
        
        [Fact]
        public async Task Update_Should_ReturnError_When_DescriptionHaveLengthGreaterThan250()
        {
            var result = await _aggregation.UpdateAsync(_fixture.CreateWithLength(20), 
                _fixture.CreateWithLength(50),
                _fixture.CreateWithLength(251), _fixture.Create<bool>());

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(InvalidDescription);
        }
        
        [Fact]
        public async Task Update_Should_ReturnOk_When_DescriptionIsNull()
        {
            var name = _fixture.CreateWithLength(20);
            var displayName = _fixture.CreateWithLength(50);
            var isEnable = _fixture.Create<bool>();
            var result = await _aggregation.UpdateAsync(name, 
                displayName,
                null, isEnable);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Name.Should().Be(name);
            _state.DisplayName.Should().Be(displayName);
            _state.Description.Should().BeNullOrEmpty();
            _state.IsEnable.Should().Be(isEnable);
        }
        
        [Fact]
        public async Task Update_Should_ReturnOk_When_DescriptionIsNotNull()
        {
            var name = _fixture.CreateWithLength(20);
            var displayName = _fixture.Create<string>();
            var description = _fixture.Create<string>();
            var isEnable = _fixture.Create<bool>();
            
            var result = await _aggregation.UpdateAsync(name, displayName,
                description, isEnable);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _state.Name.Should().Be(name);
            _state.DisplayName.Should().Be(displayName);
            _state.Description.Should().Be(description);
            _state.IsEnable.Should().Be(isEnable);
        }
    }
}
