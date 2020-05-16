using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToUpdateRoleReplay : IMapper<Result, Proto.UpdateRoleReplay>
    {
        private readonly IMapper<Domain.Common.Role, Proto.Role> _mapper;

        public ResultToUpdateRoleReplay(IMapper<Domain.Common.Role, Proto.Role> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public UpdateRoleReplay Map(Result source)
        {
            return new UpdateRoleReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.Role)source.Value)
            };
        }
    }
}