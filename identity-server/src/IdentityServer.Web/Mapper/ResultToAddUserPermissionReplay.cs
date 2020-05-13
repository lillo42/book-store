using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToAddUserPermissionReplay : IMapper<Result, Proto.AddUserPermissionReplay>
    {
        public AddUserPermissionReplay Map(Result source)
        {
            return new AddUserPermissionReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}