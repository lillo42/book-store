using System;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityServer.Application.Operation.Role;
using IdentityServer.Application.Request.Role;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Mapper;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;

namespace IdentityServer.Web.Services
{
    public class RoleService : Roles.RolesBase
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IServiceProvider provider, 
            ILogger<RoleService> logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public override async Task<CreateRoleReplay> CreateRole(CreateRoleRequest request, ServerCallContext context)
        {
            using (MiniProfiler.Current.Step(nameof(CreateRole)))
            {
                _logger.LogInformation($"Going to execute {nameof(RoleCreateOperation)}");
                var operation = _provider.GetRequiredService<RoleCreateOperation>();
                var result = await operation.ExecuteAsync(new RoleCreate
                {
                    Name = request.Name,
                    Description = request.Description,
                    DisplayName = request.DisplayName,
                }).ConfigureAwait(false);

                return _provider.GetService<IMapper<Result, CreateRoleReplay>>()
                    .Map(result);
            }
        }
    }
}