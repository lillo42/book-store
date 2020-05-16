using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToGetClientByIdReplay : IMapper<Result, Proto.GetClientByIdReplay>
    {
        private readonly IMapper<Domain.Common.Client, Proto.Client> _mapper;

        public ResultToGetClientByIdReplay(IMapper<Domain.Common.Client, Proto.Client> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public GetClientByIdReplay Map(Result source)
        {
            return new GetClientByIdReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.Client)source.Value)
            };
        }
    }
}