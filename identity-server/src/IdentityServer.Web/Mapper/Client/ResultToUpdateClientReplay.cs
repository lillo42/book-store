using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToUpdateClientReplay : IMapper<Result, Proto.UpdateClientReplay>
    {
        private readonly IMapper<Domain.Common.Client, Proto.Client> _mapper;

        public ResultToUpdateClientReplay(IMapper<Domain.Common.Client, Proto.Client> mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public UpdateClientReplay Map(Result source)
        {
            return new UpdateClientReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
                Value = _mapper.Map((Domain.Common.Client)source.Value)
            };
        }
    }
}