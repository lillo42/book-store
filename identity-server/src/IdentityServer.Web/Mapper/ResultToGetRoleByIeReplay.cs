using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToGetRoleByIeReplay : IMapper<Result, Proto.GetRoleByIeReplay>
    {
        private readonly IMapper<Domain.Common.Role, Proto.Role> _mapper;

        public ResultToGetRoleByIeReplay(IMapper<Domain.Common.Role, Proto.Role> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public GetRoleByIeReplay Map(Result source)
        {
            return new GetRoleByIeReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.Role)source.Value)
            };
        }
    }
}