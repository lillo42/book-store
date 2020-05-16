using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Resource = IdentityServer.Domain.Common.Resource;

namespace IdentityServer.Web.Mapper
{
    public class ResultToCreateUserReplay : IMapper<Result, Proto.CreateUserReplay>
    {
        private readonly IMapper<Domain.Common.User, Proto.User> _user;

        public ResultToCreateUserReplay(IMapper<Domain.Common.User, Proto.User> resource)
        {
            _user = resource ?? throw new ArgumentNullException(nameof(resource));
        }

        public CreateUserReplay Map(Result source)
        {
            return new CreateUserReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _user.Map((Domain.Common.User)source.Value)
            };
        }
    }
}