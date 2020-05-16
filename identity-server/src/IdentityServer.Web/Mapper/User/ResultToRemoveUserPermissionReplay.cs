using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToRemoveUserPermissionReplay : IMapper<Result, Proto.RemoveUserPermissionReplay>
    {
        public RemoveUserPermissionReplay Map(Result source)
        {
            return new RemoveUserPermissionReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}