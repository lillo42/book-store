using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper.Client
{
    public class ResultToAddClientResourceReplay : IMapper<Result, Proto.AddClientResourceReplay>
    {
        public AddClientResourceReplay Map(Result source)
        {
            return new AddClientResourceReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}