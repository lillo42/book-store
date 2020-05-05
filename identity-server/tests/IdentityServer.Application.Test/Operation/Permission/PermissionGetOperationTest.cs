using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.Permission;
using IdentityServer.Application.Request.Permission;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using static IdentityServer.Domain.DomainError.PermissionError;

namespace IdentityServer.Application.Test.Operation.Permission
{
    public class PermissionGetOperationTest
    {
        private readonly IReadOnlyPermissionRepository _repository;
        private readonly ILogger<PermissionGetOperation> _logger;
        private readonly PermissionGetOperation _operation;
        private readonly Fixture _fixture;

        public PermissionGetOperationTest()
        {
            _fixture = new Fixture();
            
            _repository = Substitute.For<IReadOnlyPermissionRepository>();
            _logger = Substitute.For<ILogger<PermissionGetOperation>>();
            
            _operation = new PermissionGetOperation(_repository, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_NotFound()
        {
            var request = _fixture.Create<PermissionGetById>();
            
            _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Domain.Common.Permission>(null));

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
            var request = _fixture.Create<PermissionGetById>();
            var entity = _fixture.Create<Domain.Common.Permission>();

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