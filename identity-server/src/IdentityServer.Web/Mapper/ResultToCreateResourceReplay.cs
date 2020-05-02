using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;
using Resource = IdentityServer.Domain.Common.Resource;

namespace IdentityServer.Web.Mapper
{
    public class ResultToCreateResourceReplay : IMapper<Result, Proto.CreateResourceReplay>
    {
        private readonly IMapper<Domain.Common.Resource, Proto.Resource> _resource;

        public ResultToCreateResourceReplay(IMapper<Resource, Proto.Resource> resource)
        {
            _resource = resource ?? throw new ArgumentNullException(nameof(resource));
        }

        public CreateResourceReplay Map(Result source)
        {
            return new CreateResourceReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode,
                Description = source.Description,
                Value = _resource.Map((Domain.Common.Resource)source.Value)
            };
        }
    }
}