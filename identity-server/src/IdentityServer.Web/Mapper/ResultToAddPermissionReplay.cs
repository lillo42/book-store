using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToAddPermissionReplay : IMapper<Result, Proto.AddRolePermissionReplay>
    {
        public AddRolePermissionReplay Map(Result source)
        {
            return new AddRolePermissionReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}