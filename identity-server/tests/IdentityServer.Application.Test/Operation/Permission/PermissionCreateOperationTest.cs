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
    public class PermissionCreateOperationTest
    {
        private readonly IPermissionAggregationStore _store;
        private readonly ILogger<PermissionCreateOperation> _logger;
        private readonly PermissionCreateOperation _operation;
        private readonly Fixture _fixture;

        public PermissionCreateOperationTest()
        {
            _fixture = new Fixture();
            
            _store = Substitute.For<IPermissionAggregationStore>();
            _logger = Substitute.For<ILogger<PermissionCreateOperation>>();
            
            _operation = new PermissionCreateOperation(_store, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_CreateReturnError()
        {
            var request = _fixture.Create<PermissionCreate>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IPermissionAggregationRoot>();

            root.CreateAsync(request.Name, request.DisplayName, request.Description)
                .Returns(error);
            
            _store.Create()
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().Be(error);
            
            _store
                .Received(1)
                .Create();

            var _ = root
                .Received(1)
                .CreateAsync(request.Name, request.DisplayName, request.Description);
        }
        
        [Fact]
        public async Task Execute_Should_ReturnError_When_Throw()
        {
            var request = _fixture.Create<PermissionCreate>();
            var root = Substitute.For<IPermissionAggregationRoot>();

            root.CreateAsync(request.Name, request.DisplayName, request.Description)
                .Returns(Result.Ok());

            _store.Create()
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
            
            _store
                .Received(1)
                .Create();

            var _ = root
                .Received(1)
                .CreateAsync(request.Name, request.DisplayName, request.Description);

            var __ =_store
                .Received(1)
                .SaveAsync(root, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Execute_Should_ReturnOK()
        {
            var request = _fixture.Create<PermissionCreate>();
            
            var root = Substitute.For<IPermissionAggregationRoot>();

            root.CreateAsync(request.Name, request.DisplayName, request.Description)
                .Returns(Result.Ok());

            var permission = _fixture.Create<Domain.Common.Permission>();
            
            root.State
                .Returns(new PermissionState(permission));
            
            _store.Create()
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(permission);

            _store
                .Received(1)
                .Create();
            
            var _ = root
                .Received(1)
                .CreateAsync(request.Name, request.DisplayName, request.Description);

            var __ =root
                .Received(1)
                .State;
        }
    }
}