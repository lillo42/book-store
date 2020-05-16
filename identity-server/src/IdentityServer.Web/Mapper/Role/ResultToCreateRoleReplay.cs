using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToCreateRoleReplay : IMapper<Result, Proto.CreateRoleReplay>
    {
        private readonly IMapper<Domain.Common.Role, Proto.Role> _mapper;

        public ResultToCreateRoleReplay(IMapper<Domain.Common.Role, Proto.Role> resource)
        {
            _mapper = resource ?? throw new ArgumentNullException(nameof(resource));
        }

        public CreateRoleReplay Map(Result source)
        {
            return new CreateRoleReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.Role)source.Value)
            };
        }
    }
}