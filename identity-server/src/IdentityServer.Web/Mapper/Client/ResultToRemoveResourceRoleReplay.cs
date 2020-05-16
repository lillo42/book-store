using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper.Client
{
    public class ResultToRemoveClientResourceReplay : IMapper<Result, Proto.RemoveClientResourceReplay>
    {
        public RemoveClientResourceReplay Map(Result source)
        {
            return new RemoveClientResourceReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}