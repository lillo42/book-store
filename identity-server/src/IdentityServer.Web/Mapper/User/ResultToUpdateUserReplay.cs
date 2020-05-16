using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToUpdateUserReplay : IMapper<Result, Proto.UpdateUserReplay>
    {
        private readonly IMapper<Domain.Common.User, Proto.User> _mapper;

        public ResultToUpdateUserReplay(IMapper<Domain.Common.User, Proto.User> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public UpdateUserReplay Map(Result source)
        {
            return new UpdateUserReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.User)source.Value)
            };
        }
    }
}