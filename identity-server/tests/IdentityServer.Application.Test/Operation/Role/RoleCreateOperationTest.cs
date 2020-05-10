using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.Role;
using IdentityServer.Application.Request.Role;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Role;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace IdentityServer.Application.Test.Operation.Role
{
    public class RoleCreateOperationTest
    {
        private readonly IRoleAggregationStore _store;
        private readonly ILogger<RoleCreateOperation> _logger;
        private readonly RoleCreateOperation _operation;
        private readonly Fixture _fixture;

        public RoleCreateOperationTest()
        {
            _fixture = new Fixture();
            
            _store = Substitute.For<IRoleAggregationStore>();
            _logger = Substitute.For<ILogger<RoleCreateOperation>>();
            
            _operation = new RoleCreateOperation(_store, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_CreateReturnError()
        {
            var request = _fixture.Create<RoleCreate>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IRoleAggregationRoot>();

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
            var request = _fixture.Create<RoleCreate>();
            var root = Substitute.For<IRoleAggregationRoot>();

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
            var request = _fixture.Create<RoleCreate>();
            
            var root = Substitute.For<IRoleAggregationRoot>();

            root.CreateAsync(request.Name, request.DisplayName, request.Description)
                .Returns(Result.Ok());

            var Role = _fixture.Create<Domain.Common.Role>();
            
            root.State
                .Returns(new RoleState(Role));
            
            _store.Create()
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(Role);

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