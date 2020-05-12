using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.User;
using IdentityServer.Application.Request.User;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using static IdentityServer.Domain.DomainError.UserError;

namespace IdentityServer.Application.Test.Operation.User
{
    public class UserGetOperationTest
    {
        private readonly IReadOnlyUserRepository _repository;
        private readonly ILogger<UserGetOperation> _logger;
        private readonly UserGetOperation _operation;
        private readonly Fixture _fixture;

        public UserGetOperationTest()
        {
            _fixture = new Fixture();
            
            _repository = Substitute.For<IReadOnlyUserRepository>();
            _logger = Substitute.For<ILogger<UserGetOperation>>();
            
            _operation = new UserGetOperation(_repository, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_NotFound()
        {
            var request = _fixture.Create<UserGetById>();
            
            _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Domain.Common.User>(null));

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
            var request = _fixture.Create<UserGetById>();
            var entity = _fixture.Create<Domain.Common.User>();

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