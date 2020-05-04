using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Resource = IdentityServer.Domain.Common.Resource;

namespace IdentityServer.Web.Mapper
{
    public class ResultToGetResourceByIeReplay : IMapper<Result, Proto.GetResourceByIeReplay>
    {
        private readonly IMapper<Domain.Common.Resource, Proto.Resource> _resource;

        public ResultToGetResourceByIeReplay(IMapper<Resource, Proto.Resource> resource)
        {
            _resource = resource ?? throw new ArgumentNullException(nameof(resource));
        }

        public GetResourceByIeReplay Map(Result source)
        {
            return new GetResourceByIeReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _resource.Map((Domain.Common.Resource)source.Value)
            };
        }
    }
}