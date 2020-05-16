using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToRemoveUserRoleReplay : IMapper<Result, Proto.RemoveUserRoleReplay>
    {
        public RemoveUserRoleReplay Map(Result source)
        {
            return new RemoveUserRoleReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}