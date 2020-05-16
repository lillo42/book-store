using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToCreateClientReplay : IMapper<Result, Proto.CreateClientReplay>
    {
        private readonly IMapper<Domain.Common.Client, Proto.Client> _mapper;

        public ResultToCreateClientReplay(IMapper<Domain.Common.Client, Proto.Client> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public CreateClientReplay Map(Result source)
        {
            return new CreateClientReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.Client)source.Value)
            };
        }
    }
}