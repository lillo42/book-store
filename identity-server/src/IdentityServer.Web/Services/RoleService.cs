using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityServer.Application.Operation.Role;
using IdentityServer.Application.Request.Role;
using IdentityServer.Domain;
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
            _logger.LogInformation($"Going to execute {nameof(RoleCreateOperation)}");
            var operation = _provider.GetRequiredService<RoleCreateOperation>();
            Result result;
            using (MiniProfiler.Current.Step(nameof(CreateRole)))
            {
                result = await operation.ExecuteAsync(new RoleCreate
                {
                    Name = request.Name,
                    Description = request.Description,
                    DisplayName = request.DisplayName,
                }).ConfigureAwait(false);
            }
            
            return _provider.GetService<IMapper<Result, CreateRoleReplay>>()
                .Map(result);
        }

        public override async Task<UpdateRoleReplay> UpdateRole(UpdateRoleRequest request, ServerCallContext context)
        {
            Result result;
            if (Guid.TryParse(request.Id, out var id))
            {
                _logger.LogInformation($"Going to execute {nameof(RoleUpdateOperation)}");
                    
                var operation = _provider.GetRequiredService<RoleUpdateOperation>();
                using (MiniProfiler.Current.Step(nameof(RoleUpdateOperation)))
                {
                    result = await operation.ExecuteAsync(new RoleUpdate
                    {
                        Id = id,
                        Name = request.Name,
                        DisplayName = request.DisplayName,
                        Description = request.Description
                    }).ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.RoleError.InvalidId;
            }
                
            return _provider.GetService<IMapper<Result, UpdateRoleReplay>>()
                .Map(result);
        }

        public override async Task<GetRoleByIeReplay> GetRoleById(GetRoleByIdRequest request, ServerCallContext context)
        {
            Result result;
            if (Guid.TryParse(request.Id, out var id))
            {
                _logger.LogInformation($"Going to execute {nameof(RoleGetOperation)}");
                    
                var operation = _provider.GetRequiredService<RoleGetOperation>();
                using (MiniProfiler.Current.Step(nameof(RoleGetOperation)))
                {
                    result = await operation.ExecuteAsync(new RoleGetById
                        {
                            Id = id
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.PermissionError.InvalidId;
            }

            return _provider.GetService<IMapper<Result, GetRoleByIeReplay>>()
                .Map(result);
        }

        public override async Task GetRoles(GetRolesRequest request, IServerStreamWriter<Role> responseStream, ServerCallContext context)
        {
            _logger.LogInformation($"Going to execute {nameof(RoleGetAllOperation)}");
                    
            var operation = _provider.GetRequiredService<RoleGetAllOperation>();
            var result = await operation.ExecuteAsync(new RoleGetAll())
                .ConfigureAwait(false);

            var mapper = _provider.GetRequiredService<IMapper<Domain.Common.Role, Role>>();
            if (result is OkResult<IAsyncEnumerable<Domain.Common.Role>> ok)
            {
                await foreach (var permission in ok.Value
                    .WithCancellation(context.CancellationToken)
                    .ConfigureAwait(false))
                {
                    await responseStream.WriteAsync(mapper.Map(permission))
                        .ConfigureAwait(false);
                }
            }
        }

        public override async Task<AddPermissionReplay> AddPermission(AddPermissionRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var roleId) 
               && Guid.TryParse(request.PermissionId, out var permissionId))
            {
                _logger.LogInformation($"Going to execute {nameof(RoleAddPermissionOperation)}");
                var operation = _provider.GetRequiredService<RoleAddPermissionOperation>();
                using (MiniProfiler.Current.Step(nameof(RoleAddPermissionOperation)))
                {
                    result = await operation.ExecuteAsync(new RoleAddPermission
                    {
                        Id = roleId,
                        PermissionId = permissionId
                    })
                    .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.PermissionError.InvalidId;
            }
            
            
            return _provider.GetService<IMapper<Result, AddPermissionReplay>>()
                .Map(result);
        }

        public override async Task<RemovePermissionReplay> RemovePermission(RemovePermissionRequest request, ServerCallContext context)
        {
            Result result;
            if(Guid.TryParse(request.Id, out var roleId) 
               && Guid.TryParse(request.PermissionId, out var permissionId))
            {
                _logger.LogInformation($"Going to execute {nameof(RoleRemovePermissionOperation)}");
                var operation = _provider.GetRequiredService<RoleRemovePermissionOperation>();
                using (MiniProfiler.Current.Step(nameof(RoleRemovePermissionOperation)))
                {
                    result = await operation.ExecuteAsync(new RoleRemovePermission
                        {
                            Id = roleId,
                            PermissionId = permissionId
                        })
                        .ConfigureAwait(false);
                }
            }
            else
            {
                result = DomainError.PermissionError.InvalidId;
            }
            
            
            return _provider.GetService<IMapper<Result, RemovePermissionReplay>>()
                .Map(result);
        }
    }
}