using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IdentityServer.Application.Operation.Resource;
using IdentityServer.Application.Request.Resource;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Domain.Abstractions.Resource;
using IdentityServer.Domain.Common;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace IdentityServer.Application.Test.Operation
{
    public class ResourceUpdateOperationTest
    {
        private readonly IResourceAggregationStore _store;
        private readonly ILogger<ResourceUpdateOperation> _logger;
        private readonly ResourceUpdateOperation _operation;
        private readonly Fixture _fixture;

        public ResourceUpdateOperationTest()
        {
            _fixture = new Fixture();
            
            _store = Substitute.For<IResourceAggregationStore>();
            _logger = Substitute.For<ILogger<ResourceUpdateOperation>>();
            
            _operation = new ResourceUpdateOperation(_store, _logger);
        }

        [Fact]
        public async Task Execute_Should_ReturnError_When_ResourceNotFound()
        {
            var request = _fixture.Create<ResourceUpdate>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IResourceAggregationRoot>();

            root.Update(request.Name, request.DisplayName, request.Description, request.IsEnable)
                .Returns(error);
            
            _store.GetAsync(request.Id)
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().Be(error);
            
            var __ = _store
                .Received(1)
                .GetAsync(request.Id);

            root
                .Received(1)
                .Update(request.Name, request.DisplayName, request.Description, request.IsEnable);
        }

        
        [Fact]
        public async Task Execute_Should_ReturnError_When_UpdateReturnError()
        {
            var request = _fixture.Create<ResourceUpdate>();
            var error = Result.Fail(_fixture.Create<string>(), _fixture.Create<string>());
            
            var root = Substitute.For<IResourceAggregationRoot>();

            root.Update(request.Name, request.DisplayName, request.Description, request.IsEnable)
                .Returns(error);
            
            _store.GetAsync(request.Id)
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Should().Be(error);
            
            var __ = _store
                .Received(1)
                .GetAsync(request.Id);

            root
                .Received(1)
                .Update(request.Name, request.DisplayName, request.Description, request.IsEnable);
        }
        
        [Fact]
        public async Task Execute_Should_ReturnError_When_Throw()
        {
            var request = _fixture.Create<ResourceUpdate>();
            var root = Substitute.For<IResourceAggregationRoot>();

            root.Update(request.Name, request.DisplayName, request.Description, request.IsEnable)
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
            
            var __ =_store
                .Received(1)
                .GetAsync(request.Id);

            root
                .Received(1)
                .Update(request.Name, request.DisplayName, request.Description, request.IsEnable);

            var _ =_store
                .Received(1)
                .SaveAsync(root, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Execute_Should_ReturnOK()
        {
            var request = _fixture.Create<ResourceUpdate>();
            
            var root = Substitute.For<IResourceAggregationRoot>();

            root.Update(request.Name, request.DisplayName, request.Description, request.IsEnable)
                .Returns(Result.Ok());

            var resource = _fixture.Create<Resource>();
            
            root.State
                .Returns(new ResourceState(resource));
            
            _store.GetAsync(request.Id)
                .Returns(root);

            var result = await _operation.ExecuteAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().Be(resource);

            var __ =_store
                .Received(1)
                .GetAsync(request.Id);
            
            root
                .Received(1)
                .Update(request.Name, request.DisplayName, request.Description, request.IsEnable);

            var _ =root
                .Received(1)
                .State;
        }
    }
}