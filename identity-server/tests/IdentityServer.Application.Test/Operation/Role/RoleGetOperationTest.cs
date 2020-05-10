using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.Role;
using IdentityServer.Application.Request.Role;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using static IdentityServer.Domain.DomainError.RoleError;

namespace IdentityServer.Application.Test.Operation.Role
{
    public class RoleGetOperationTest
    {
        private readonly IReadOnlyRoleRepository _repository;
        private readonly ILogger<RoleGetOperation> _logger;
        private readonly RoleGetOperation _operation;
        private readonly Fixture _fixture;

        public RoleGetOperationTest()
        {
            _fixture = new Fixture();
            
            _repository = Substitute.For<IReadOnlyRoleRepository>();
            _logger = Substitute.For<ILogger<RoleGetOperation>>();
            
            _operation = new RoleGetOperation(_repository, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_NotFound()
        {
            var request = _fixture.Create<RoleGetById>();
            
            _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Domain.Common.Role>(null));

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
            var request = _fixture.Create<RoleGetById>();
            var entity = _fixture.Create<Domain.Common.Role>();

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