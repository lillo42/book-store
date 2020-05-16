using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.Resource;
using IdentityServer.Application.Request.Resource;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Resource;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace IdentityServer.Application.Test.Operation.Resource
{
    public class ResourceCreateOperationTest
    {
        private readonly IResourceAggregationStore _store;
        private readonly ILogger<ResourceCreateOperation> _logger;
        private readonly ResourceCreateOperation _operation;
        private readonly Fixture _fixture;

        public ResourceCreateOperationTest()
        {
            _fixture = new Fixture();
            
            _store = Substitute.For<IResourceAggregationStore>();
            _logger = Substitute.For<ILogger<ResourceCreateOperation>>();
            
            _operation = new ResourceCreateOperation(_store, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_CreateReturnError()
        {
            var request = _fixture.Create<ResourceCreate>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IResourceAggregationRoot>();

            root.CreateAsync(request.Name, request.DisplayName, request.Description, request.IsEnable)
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

            root
                .Received(1)
                .CreateAsync(request.Name, request.DisplayName, request.Description, request.IsEnable);
        }
        
        [Fact]
        public async Task Execute_Should_ReturnError_When_Throw()
        {
            var request = _fixture.Create<ResourceCreate>();
            var root = Substitute.For<IResourceAggregationRoot>();

            root.CreateAsync(request.Name, request.DisplayName, request.Description, request.IsEnable)
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

            var __ = root
                .Received(1)
                .CreateAsync(request.Name, request.DisplayName, request.Description, request.IsEnable);

            var _ =_store
                .Received(1)
                .SaveAsync(root, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Execute_Should_ReturnOK()
        {
            var request = _fixture.Create<ResourceCreate>();
            
            var root = Substitute.For<IResourceAggregationRoot>();

            root.CreateAsync(request.Name, request.DisplayName, request.Description, request.IsEnable)
                .Returns(Result.Ok());

            var resource = _fixture.Create<Domain.Common.Resource>();
            
            root.State
                .Returns(new ResourceState(resource));
            
            _store.Create()
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(resource);

            _store
                .Received(1)
                .Create();
            
            var __ = root
                .Received(1)
                .CreateAsync(request.Name, request.DisplayName, request.Description, request.IsEnable);

            var _ =root
                .Received(1)
                .State;
        }
    }
}