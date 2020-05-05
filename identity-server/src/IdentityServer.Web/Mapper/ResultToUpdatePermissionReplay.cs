using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Resource = IdentityServer.Domain.Common.Resource;

namespace IdentityServer.Web.Mapper
{
    public class ResultToUpdatePermissionReplay : IMapper<Result, Proto.UpdatePermissionReplay>
    {
        private readonly IMapper<Domain.Common.Permission, Proto.Permission> _mapper;

        public ResultToUpdatePermissionReplay(IMapper<Domain.Common.Permission, Proto.Permission> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public UpdatePermissionReplay Map(Result source)
        {
            return new UpdatePermissionReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.Permission)source.Value)
            };
        }
    }
}