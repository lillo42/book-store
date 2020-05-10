using System;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Proto;

namespace IdentityServer.Web.Mapper
{
    public class ResultToAddPermissionReplay : IMapper<Result, Proto.AddPermissionReplay>
    {
        public AddPermissionReplay Map(Result source)
        {
            return new AddPermissionReplay
            {
                IsSuccess = source.IsSuccess,
                ErrorCode = source.ErrorCode ?? string.Empty,
                Description = source.Description ?? string.Empty,
            };
        }
    }
}