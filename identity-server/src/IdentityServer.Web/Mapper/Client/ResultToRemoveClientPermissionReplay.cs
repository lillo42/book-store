using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper.Client
{
    public class ResultToRemoveClientPermissionReplay : IMapper<Result, Proto.RemoveClientPermissionReplay>
    {
        public RemoveClientPermissionReplay Map(Result source)
        {
            return new RemoveClientPermissionReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}