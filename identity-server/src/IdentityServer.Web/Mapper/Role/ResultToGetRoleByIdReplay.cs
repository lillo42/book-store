using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToGetRoleByIdReplay : IMapper<Result, Proto.GetRoleByIdReplay>
    {
        private readonly IMapper<Domain.Common.Role, Proto.Role> _mapper;

        public ResultToGetRoleByIdReplay(IMapper<Domain.Common.Role, Proto.Role> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public GetRoleByIdReplay Map(Result source)
        {
            return new GetRoleByIdReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.Role)source.Value)
            };
        }
    }
}