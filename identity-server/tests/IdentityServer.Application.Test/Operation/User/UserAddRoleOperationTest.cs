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
    public class UserAddRoleOperationTest
    {
        private readonly ILogger<UserAddRoleOperation> _logger;
        private readonly IUserAggregationStore _store;
        private readonly UserAddRoleOperation _operation;
        private readonly Fixture _fixture;

        public UserAddRoleOperationTest()
        {
            _fixture = new Fixture();
            _store = Substitute.For<IUserAggregationStore>();
            _logger = Substitute.For<ILogger<UserAddRoleOperation>>();
            
            _operation = new UserAddRoleOperation(_store, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_UserNotFound()
        {
            var request = _fixture.Create<UserAddRole>();
            
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
            var request = _fixture.Create<UserAddRole>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IUserAggregationRoot>();

            root.AddRoleAsync(Arg.Is<Domain.Common.Role>(x => x.Id == request.RoleId))
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
                .AddRoleAsync(Arg.Is<Domain.Common.Role>(x => x.Id == request.RoleId));
        }
        
        [Fact]
        public async Task Execute_Should_ReturnError_When_Throw()
        {
            var request = _fixture.Create<UserAddRole>();
            var root = Substitute.For<IUserAggregationRoot>();

            root.AddRoleAsync(Arg.Is<Domain.Common.Role>(x => x.Id == request.RoleId))
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
                .AddRoleAsync(Arg.Is<Domain.Common.Role>(x => x.Id == request.RoleId));

            var ___ =_store
                .Received(1)
                .SaveAsync(root, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Execute_Should_ReturnOK()
        {
            var request = _fixture.Create<UserAddRole>();
            
            var root = Substitute.For<IUserAggregationRoot>();

            root.AddRoleAsync(Arg.Is<Domain.Common.Role>(x => x.Id == request.RoleId))
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
                .AddRoleAsync(Arg.Is<Domain.Common.Role>(x => x.Id == request.RoleId));

            var ___ =root
                .Received(1)
                .State;
        }
    }
}