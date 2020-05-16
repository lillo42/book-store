using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Resource = IdentityServer.Domain.Common.Resource;

namespace IdentityServer.Web.Mapper
{
    public class ResultToGetResourceByIdReplay : IMapper<Result, Proto.GetResourceByIdReplay>
    {
        private readonly IMapper<Domain.Common.Resource, Proto.Resource> _resource;

        public ResultToGetResourceByIdReplay(IMapper<Resource, Proto.Resource> resource)
        {
            _resource = resource ?? throw new ArgumentNullException(nameof(resource));
        }

        public GetResourceByIdReplay Map(Result source)
        {
            return new GetResourceByIdReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _resource.Map((Domain.Common.Resource)source.Value)
            };
        }
    }
}