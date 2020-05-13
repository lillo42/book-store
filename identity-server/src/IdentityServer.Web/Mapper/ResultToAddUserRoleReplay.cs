using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToAddUserRoleReplay : IMapper<Result, Proto.AddUserRoleReplay>
    {
        public AddUserRoleReplay Map(Result source)
        {
            return new AddUserRoleReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}