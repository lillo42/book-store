using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper.Client
{
    public class ResultToAddClientPermissionReplay : IMapper<Result, Proto.AddClientPermissionReplay>
    {
        public AddClientPermissionReplay Map(Result source)
        {
            return new AddClientPermissionReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}