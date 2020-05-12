using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.User;
using IdentityServer.Application.Request.User;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.User;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace IdentityServer.Application.Test.Operation.User
{
    public class UserAddPermissionOperationTest
    {
        private readonly ILogger<UserAddPermissionOperation> _logger;
        private readonly IUserAggregationStore _store;
        private readonly UserAddPermissionOperation _operation;
        private readonly Fixture _fixture;

        public UserAddPermissionOperationTest()
        {
            _fixture = new Fixture();
            _store = Substitute.For<IUserAggregationStore>();
            _logger = Substitute.For<ILogger<UserAddPermissionOperation>>();
            
            _operation = new UserAddPermissionOperation(_store, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_UserNotFound()
        {
            var request = _fixture.Create<UserAddPermission>();
            
            _store.GetAsync(request.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IUserAggregationRoot>(null));
            
            var result = await _operation.ExecuteAsync(request, CancellationToken.None);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().Be(DomainError.UserError.NotFound);
            
            var _ = _store
                .Received(1)
                .GetAsync(request.Id);
        }
        
        [Fact]
        public async Task Execute_Should_ReturnError_When_UpdateReturnError()
        {
            var request = _fixture.Create<UserAddPermission>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IUserAggregationRoot>();

            root.AddPermissionAsync(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId))
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
                .AddPermissionAsync(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId));
        }
        
        [Fact]
        public async Task Execute_Should_ReturnError_When_Throw()
        {
            var request = _fixture.Create<UserAddPermission>();
            var root = Substitute.For<IUserAggregationRoot>();

            root.AddPermissionAsync(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId))
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
                .AddPermissionAsync(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId));

            var ___ =_store
                .Received(1)
                .SaveAsync(root, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Execute_Should_ReturnOK()
        {
            var request = _fixture.Create<UserAddPermission>();
            
            var root = Substitute.For<IUserAggregationRoot>();

            root.AddPermissionAsync(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId))
                .Returns(Result.Ok());

            var user = _fixture.Create<Domain.Common.User>();
            
            root.State
                .Returns(new UserState(user));
            
            _store.GetAsync(request.Id)
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(user);

            var _ =_store
                .Received(1)
                .GetAsync(request.Id);
            
            var __ =root
                .Received(1)
                .AddPermissionAsync(Arg.Is<Domain.Common.Permission>(x => x.Id == request.PermissionId));

            var ___ =root
                .Received(1)
                .State;
        }
    }
}