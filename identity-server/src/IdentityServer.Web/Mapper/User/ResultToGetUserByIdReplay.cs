using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToGetUserByIdReplay : IMapper<Result, Proto.GetUserByIdReplay>
    {
        private readonly IMapper<Domain.Common.User, Proto.User> _mapper;

        public ResultToGetUserByIdReplay(IMapper<Domain.Common.User, Proto.User> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public GetUserByIdReplay Map(Result source)
        {
            return new GetUserByIdReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.User)source.Value)
            };
        }
    }
}