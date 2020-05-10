using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.Role;
using IdentityServer.Application.Request.Role;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Role;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace IdentityServer.Application.Test.Operation.Role
{
    public class RoleUpdateOperationTest
    {
        private readonly IRoleAggregationStore _store;
        private readonly ILogger<RoleUpdateOperation> _logger;
        private readonly RoleUpdateOperation _operation;
        private readonly Fixture _fixture;

        public RoleUpdateOperationTest()
        {
            _fixture = new Fixture();
            
            _store = Substitute.For<IRoleAggregationStore>();
            _logger = Substitute.For<ILogger<RoleUpdateOperation>>();
            
            _operation = new RoleUpdateOperation(_store, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_RoleNotFound()
        {
            var request = _fixture.Create<RoleUpdate>();
            
            var root = Substitute.For<IRoleAggregationRoot>();

            _store.GetAsync(request.Id)
                .Returns(Task.FromResult<IRoleAggregationRoot>(null));

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().Be(DomainError.RoleError.NotFound);
            
            var _ = _store
                .Received(1)
                .GetAsync(request.Id);
        }

        
        [Fact]
        public async Task Execute_Should_ReturnError_When_UpdateReturnError()
        {
            var request = _fixture.Create<RoleUpdate>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IRoleAggregationRoot>();

            root.UpdateAsync(request.Name, request.DisplayName, request.Description)
                .Returns(error);
            
            _store.GetAsync(request.Id)
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().Be(error);
            
            var _ = _store
                .Received(1)
                .GetAsync(request.Id);

            var __ =root
                .Received(1)
                .UpdateAsync(request.Name, request.DisplayName, request.Description);
        }
        
        [Fact]
        public async Task Execute_Should_ReturnError_When_Throw()
        {
            var request = _fixture.Create<RoleUpdate>();
            var root = Substitute.For<IRoleAggregationRoot>();

            root.UpdateAsync(request.Name, request.DisplayName, request.Description)
                .Returns(Result.Ok());

            _store.GetAsync(request.Id)
                .Returns(root);

            var exception = new Exception(_fixture.Create<string>())
            {
                HResult = _fixture.Create<int>()
            };

            _store.SaveAsync(root, Arg.Any<CancellationToken>())
                .Throws(exception);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().BeEquivalentTo(Result.Fail(exception));
            
            var _ =_store
                .Received(1)
                .GetAsync(request.Id);

            var __ =root
                .Received(1)
                .UpdateAsync(request.Name, request.DisplayName, request.Description);

            var ___ =_store
                .Received(1)
                .SaveAsync(root, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Execute_Should_ReturnOK()
        {
            var request = _fixture.Create<RoleUpdate>();
            
            var root = Substitute.For<IRoleAggregationRoot>();

            root.UpdateAsync(request.Name, request.DisplayName, request.Description)
                .Returns(Result.Ok());

            var Role = _fixture.Create<Domain.Common.Role>();
            
            root.State
                .Returns(new RoleState(Role));
            
            _store.GetAsync(request.Id)
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(Role);

            var _ =_store
                .Received(1)
                .GetAsync(request.Id);
            
            var __ =root
                .Received(1)
                .UpdateAsync(request.Name, request.DisplayName, request.Description);

            var ___ =root
                .Received(1)
                .State;
        }
    }
}