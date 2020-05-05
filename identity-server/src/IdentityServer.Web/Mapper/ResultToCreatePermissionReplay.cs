using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Resource = IdentityServer.Domain.Common.Resource;

namespace IdentityServer.Web.Mapper
{
    public class ResultToCreatePermissionReplay : IMapper<Result, Proto.CreatePermissionReplay>
    {
        private readonly IMapper<Domain.Common.Permission, Proto.Permission> _mapper;

        public ResultToCreatePermissionReplay(IMapper<Domain.Common.Permission, Proto.Permission> resource)
        {
            _mapper = resource ?? throw new ArgumentNullException(nameof(resource));
        }

        public CreatePermissionReplay Map(Result source)
        {
            return new CreatePermissionReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.Permission)source.Value)
            };
        }
    }
}