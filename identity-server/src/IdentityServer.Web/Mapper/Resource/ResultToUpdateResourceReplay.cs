using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Resource = IdentityServer.Domain.Common.Resource;

namespace IdentityServer.Web.Mapper
{
    public class ResultToUpdateResourceReplay : IMapper<Result, Proto.UpdateResourceReplay>
    {
        private readonly IMapper<Domain.Common.Resource, Proto.Resource> _resource;

        public ResultToUpdateResourceReplay(IMapper<Resource, Proto.Resource> resource)
        {
            _resource = resource ?? throw new ArgumentNullException(nameof(resource));
        }

        public UpdateResourceReplay Map(Result source)
        {
            return new UpdateResourceReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _resource.Map((Domain.Common.Resource)source.Value)
            };
        }
    }
}