using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToGetUserByIeReplay : IMapper<Result, Proto.GetUserByIeReplay>
    {
        private readonly IMapper<Domain.Common.User, Proto.User> _mapper;

        public ResultToGetUserByIeReplay(IMapper<Domain.Common.User, Proto.User> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public GetUserByIeReplay Map(Result source)
        {
            return new GetUserByIeReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.User)source.Value)
            };
        }
    }
}