using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.Permission;
using IdentityServer.Application.Request.Permission;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Permission;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace IdentityServer.Application.Test.Operation.Permission
{
    public class PermissionUpdateOperationTest
    {
        private readonly IPermissionAggregationStore _store;
        private readonly ILogger<PermissionUpdateOperation> _logger;
        private readonly PermissionUpdateOperation _operation;
        private readonly Fixture _fixture;

        public PermissionUpdateOperationTest()
        {
            _fixture = new Fixture();
            
            _store = Substitute.For<IPermissionAggregationStore>();
            _logger = Substitute.For<ILogger<PermissionUpdateOperation>>();
            
            _operation = new PermissionUpdateOperation(_store, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_PermissionNotFound()
        {
            var request = _fixture.Create<PermissionUpdate>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IPermissionAggregationRoot>();

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

            var __ = root
                .Received(1)
                .UpdateAsync(request.Name, request.DisplayName, request.Description);
        }

        
        [Fact]
        public async Task Execute_Should_ReturnError_When_UpdateReturnError()
        {
            var request = _fixture.Create<PermissionUpdate>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IPermissionAggregationRoot>();

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
            var request = _fixture.Create<PermissionUpdate>();
            var root = Substitute.For<IPermissionAggregationRoot>();

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
            var request = _fixture.Create<PermissionUpdate>();
            
            var root = Substitute.For<IPermissionAggregationRoot>();

            root.UpdateAsync(request.Name, request.DisplayName, request.Description)
                .Returns(Result.Ok());

            var permission = _fixture.Create<Domain.Common.Permission>();
            
            root.State
                .Returns(new PermissionState(permission));
            
            _store.GetAsync(request.Id)
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(permission);

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