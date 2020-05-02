using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.Resource;
using IdentityServer.Application.Request.Resource;
using IdentityServer.Domain.Common;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

using static IdentityServer.Domain.DomainError.ResourceError;

namespace IdentityServer.Application.Test.Operation
{
    public class ResourceGetOperationTest
    {
        private readonly IReadOnlyResourceRepository _repository;
        private readonly ILogger<ResourceGetOperation> _logger;
        private readonly ResourceGetOperation _operation;
        private readonly Fixture _fixture;

        public ResourceGetOperationTest()
        {
            _fixture = new Fixture();
            
            _repository = Substitute.For<IReadOnlyResourceRepository>();
            _logger = Substitute.For<ILogger<ResourceGetOperation>>();
            
            _operation = new ResourceGetOperation(_repository, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_NotFound()
        {
            var request = _fixture.Create<ResourceGetById>();
            
            _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Resource>(null));

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().Be(NotFound);

            var _ = _repository
                .Received(1)
                .GetByIdAsync(request.Id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Execute_Should_ReturnOK()
        {
            var request = _fixture.Create<ResourceGetById>();
            var entity = _fixture.Create<Resource>();

            _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(entity));

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(entity);

            var _ = _repository
                .Received(1)
                .GetByIdAsync(request.Id, Arg.Any<CancellationToken>());
        }
    }
}