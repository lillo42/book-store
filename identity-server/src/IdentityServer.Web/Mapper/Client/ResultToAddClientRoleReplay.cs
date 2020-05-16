using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToAddClientRoleReplay : IMapper<Result, Proto.AddClientRoleReplay>
    {
        public AddClientRoleReplay Map(Result source)
        {
            return new AddClientRoleReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}