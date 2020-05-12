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
    public class RoleRemovePermissionOperationTest
    {
        private readonly ILogger<RoleRemovePermissionOperation> _logger;
        private readonly IRoleAggregationStore _store;
        private readonly RoleRemovePermissionOperation _operation;
        private readonly Fixture _fixture;

        public RoleRemovePermissionOperationTest()
        {
            _fixture = new Fixture();
            _store = Substitute.For<IRoleAggregationStore>();
            _logger = Substitute.For<ILogger<RoleRemovePermissionOperation>>();
            
            _operation = new RoleRemovePermissionOperation(_store, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_RoleNotFound()
        {
            var request = _fixture.Create<RoleRemovePermission>();
            
            _store.GetAsync(request.Id, Arg.Any<CancellationToken>())
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
            var request = _fixture.Create<RoleRemovePermission>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IRoleAggregationRoot>();

            root.RemovePermission(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId))
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
                .RemovePermission(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId));
        }
        
        [Fact]
        public async Task Execute_Should_ReturnError_When_Throw()
        {
            var request = _fixture.Create<RoleRemovePermission>();
            var root = Substitute.For<IRoleAggregationRoot>();

            root.RemovePermission(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId))
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
                .RemovePermission(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId));

            var ___ =_store
                .Received(1)
                .SaveAsync(root, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Execute_Should_ReturnOK()
        {
            var request = _fixture.Create<RoleRemovePermission>();
            
            var root = Substitute.For<IRoleAggregationRoot>();

            root.AddPermissionAsync(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId))
                .Returns(Result.Ok());

            var role = _fixture.Create<Domain.Common.Role>();
            
            root.State
                .Returns(new RoleState(role));
            
            _store.GetAsync(request.Id)
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(role);

            var _ =_store
                .Received(1)
                .GetAsync(request.Id);
            
            var __ =root
                .Received(1)
                .RemovePermission(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId));

            var ___ =root
                .Received(1)
                .State;
        }
    }
}