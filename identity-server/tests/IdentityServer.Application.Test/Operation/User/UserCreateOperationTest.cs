using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.User;
using IdentityServer.Application.Request.User;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.User;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace IdentityServer.Application.Test.Operation.User
{
    public class UserCreateOperationTest
    {
        private readonly IUserAggregationStore _store;
        private readonly ILogger<UserCreateOperation> _logger;
        private readonly UserCreateOperation _operation;
        private readonly Fixture _fixture;

        public UserCreateOperationTest()
        {
            _fixture = new Fixture();
            
            _store = Substitute.For<IUserAggregationStore>();
            _logger = Substitute.For<ILogger<UserCreateOperation>>();
            
            _operation = new UserCreateOperation(_store, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_CreateReturnError()
        {
            var request = _fixture.Create<UserCreate>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IUserAggregationRoot>();

            root.CreateAsync(request.Mail, request.Password, request.IsEnable)
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
                .CreateAsync(request.Mail, request.Password, request.IsEnable);
        }
        
        [Fact]
        public async Task Execute_Should_ReturnError_When_Throw()
        {
            var request = _fixture.Create<UserCreate>();
            var root = Substitute.For<IUserAggregationRoot>();

            root.CreateAsync(request.Mail, request.Password, request.IsEnable)
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
                .CreateAsync(request.Mail, request.Password, request.IsEnable);

            var __ =_store
                .Received(1)
                .SaveAsync(root, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Execute_Should_ReturnOK()
        {
            var request = _fixture.Create<UserCreate>();
            
            var root = Substitute.For<IUserAggregationRoot>();

            root.CreateAsync(request.Mail, request.Password, request.IsEnable)
                .Returns(Result.Ok());

            var user = _fixture.Create<Domain.Common.User>();
            
            root.State
                .Returns(new UserState(user));
            
            _store.Create()
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(user);

            _store
                .Received(1)
                .Create();
            
            var _ = root
                .Received(1)
                .CreateAsync(request.Mail, request.Password, request.IsEnable);

            var __ =root
                .Received(1)
                .State;
        }
    }
}