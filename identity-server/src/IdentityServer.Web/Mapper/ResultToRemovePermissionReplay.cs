using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToRemovePermissionReplay : IMapper<Result, Proto.RemoveRolePermissionReplay>
    {
        public RemoveRolePermissionReplay Map(Result source)
        {
            return new RemoveRolePermissionReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}