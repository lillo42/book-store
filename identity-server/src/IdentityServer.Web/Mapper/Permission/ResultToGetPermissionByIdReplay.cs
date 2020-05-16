using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToGetPermissionByIdReplay : IMapper<Result, Proto.GetPermissionByIdReplay>
    {
        private readonly IMapper<Domain.Common.Permission, Proto.Permission> _mapper;

        public ResultToGetPermissionByIdReplay(IMapper<Domain.Common.Permission, Proto.Permission> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public GetPermissionByIdReplay Map(Result source)
        {
            return new GetPermissionByIdReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.Permission)source.Value)
            };
        }
    }
}