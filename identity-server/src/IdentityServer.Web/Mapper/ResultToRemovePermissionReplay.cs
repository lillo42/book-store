using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToRemovePermissionReplay : IMapper<Result, Proto.RemovePermissionReplay>
    {
        public RemovePermissionReplay Map(Result source)
        {
            return new RemovePermissionReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}