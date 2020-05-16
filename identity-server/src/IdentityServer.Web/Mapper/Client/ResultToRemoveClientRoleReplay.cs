using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper.Client
{
    public class ResultToRemoveClientRoleReplay : IMapper<Result, Proto.RemoveClientRoleReplay>
    {
        public RemoveClientRoleReplay Map(Result source)
        {
            return new RemoveClientRoleReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}