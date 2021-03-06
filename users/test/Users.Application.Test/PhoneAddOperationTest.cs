using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Users.Application.Contracts.Request;
using Users.Application.Mapper;
using Users.Application.Operations;
using Users.Domain;
using Users.Domain.Common;
using Xunit;

using static Xunit.Assert;
using Phone = Users.Application.Contracts.Response.Phone;

namespace Users.Application.Test
{
    public class PhoneAddOperationTest
    {
        private readonly IUserAggregateStore _store;
        private readonly ILogger<PhoneAddOperation> _logger;
        private readonly IMapper<Domain.Common.Phone, Phone> _mapper;
        private readonly PhoneAddOperation _operation;
        private readonly Fixture _fixture;

        public PhoneAddOperationTest()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            
            _store = Substitute.For<IUserAggregateStore>();
            _logger = Substitute.For<ILogger<PhoneAddOperation>>();
            _mapper = Substitute.For<IMapper<Domain.Common.Phone, Phone>>();
            _operation = new PhoneAddOperation(_store, _logger, _mapper);
        }

        [Fact]
        public void Create_Should_Throw_When_StoreIsNull()
            => Throws<ArgumentNullException>(() => new PhoneAddOperation(null, _logger, _mapper));
        
        [Fact]
        public void Create_Should_Throw_When_LoggerIsNull()
            => Throws<ArgumentNullException>(() => new PhoneAddOperation(_store, null, _mapper));
        
        [Fact]
        public void Create_Should_Throw_When_MapperIsNull()
            => Throws<ArgumentNullException>(() => new PhoneAddOperation(_store, _logger, null));
        
        [Fact]
        public async Task Execute_Should_ReturnUserNotFound()
        {
            var phone = _fixture.Create<PhoneAdd>();
            _store.GetAsync(phone.UserId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((IUserAggregationRoot) null));

            var result = await _operation.ExecuteAsync(phone, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Value.Should().BeNull();
            result.ErrorCode.Should().NotBeNullOrEmpty();
            result.Description.Should().NotBeNullOrEmpty();
            result.Should().Be(DomainError.UserError.UserNotFound);

            await _store
                .Received(1)
                .GetAsync(phone.UserId, Arg.Any<CancellationToken>());

            _mapper
                .DidNotReceive()
                .Map(Arg.Any<Domain.Common.Phone>());
            
            await _store
                .DidNotReceive()
                .SaveAsync(Arg.Any<IUserAggregationRoot>(), Arg.Any<CancellationToken>());
        }


        [Fact]
        public async Task Execute_Should_ReturnError()
        {
            var phone = _fixture.Create<PhoneAdd>();
            var root = Substitute.For<IUserAggregationRoot>();

            _store.GetAsync(phone.UserId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(root));

            var fail = _fixture.Create<ErrorResult>();

            root.AddPhone(phone.Number)
                .Returns(fail);

            var result = await _operation.ExecuteAsync(phone, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Should().Be(fail);
            
            await _store
                .Received(1)
                .GetAsync(phone.UserId, Arg.Any<CancellationToken>());

            root
                .Received(1)
                .AddPhone(phone.Number);
            
            _mapper
                .DidNotReceive()
                .Map(Arg.Any<Domain.Common.Phone>());
            
            await _store
                .DidNotReceive()
                .SaveAsync(Arg.Any<IUserAggregationRoot>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Execute_Should_ReturnOk()
        {
            var phone = _fixture.Create<PhoneAdd>();
            var root = Substitute.For<IUserAggregationRoot>();
            root.State.Returns(new UserState(new User
            {
                Phones = _fixture.Create<HashSet<Domain.Common.Phone>>()
            }));

            _store.GetAsync(phone.UserId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(root));

            root.AddPhone(phone.Number)
                .Returns(Result.Ok());

            _mapper.Map(Arg.Any<Domain.Common.Phone>())
                .Returns(new Phone
                {
                    Number = phone.Number
                });

            var result = await _operation.ExecuteAsync(phone, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Should().BeOfType<OkResult<Phone>>();
            ((OkResult<Phone>) result).Value.Number.Should().Be(phone.Number);
            
            await _store
                .Received(1)
                .GetAsync(phone.UserId, Arg.Any<CancellationToken>());

            root
                .Received(1)
                .AddPhone(phone.Number);
            
            _mapper
                .Received(1)
                .Map(Arg.Any<Domain.Common.Phone>());
            
            await _store
                .Received(1)
                .SaveAsync(root, Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task Execute_Should_ReturnError_When_ThrowException()
        {
            var phone = _fixture.Create<PhoneAdd>();
            
            var exception = _fixture.Create<Exception>();
            _store.GetAsync(phone.UserId, Arg.Any<CancellationToken>())
                .Throws(exception);

            var result = await _operation.ExecuteAsync(phone, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Should().BeOfType<ErrorResult>();
            result.ErrorCode.Should().Be(exception.HResult.ToString());
            result.Description.Should().Be(exception.ToString());

            await _store
                .Received(1)
                .GetAsync(phone.UserId, Arg.Any<CancellationToken>());
            
            
            _mapper
                .DidNotReceive()
                .Map(Arg.Any<Domain.Common.Phone>());

            await _store
                .DidNotReceive()
                .SaveAsync(Arg.Any<IUserAggregationRoot>(), Arg.Any<CancellationToken>());
        }
    }
}
