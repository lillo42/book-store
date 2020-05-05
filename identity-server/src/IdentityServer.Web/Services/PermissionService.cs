using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityServer.Application.Operation.Permission;
using IdentityServer.Application.Operation.Resource;
using IdentityServer.Application.Request.Permission;
using IdentityServer.Application.Request.Resource;
using IdentityServer.Domain;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Web.Mapper;
using IdentityServer.Web.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;

namespace IdentityServer.Web.Services
{
    public class PermissionService : Permissions.PermissionsBase
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<ResourceService> _logger;

        public PermissionService(IServiceProvider provider, 
            ILogger<ResourceService> logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CreatePermissionReplay> CreatePermission(CreatePermissionRequest request, ServerCallContext context)
        {
            using (MiniProfiler.Current.Step(nameof(CreatePermission)))
            {
                _logger.LogInformation($"Going to execute {nameof(ResourceCreateOperation)}");
                var operation = _provider.GetRequiredService<PermissionCreateOperation>();
                var result = await operation.ExecuteAsync(new PermissionCreate
                {
                    Name = request.Name,
                    Description = request.Description,
                    DisplayName = request.DisplayName,
                }).ConfigureAwait(false);

                return _provider.GetService<IMapper<Result, CreatePermissionReplay>>()
                    .Map(result);
            }
        }

        public override async Task<UpdatePermissionReplay> UpdatePermission(UpdatePermissionRequest request, ServerCallContext context)
        {
            using (MiniProfiler.Current.Step(nameof(UpdatePermission)))
            {
                Result result;
                if (Guid.TryParse(request.Id, out var id))
                {
                    _logger.LogInformation($"Going to execute {nameof(PermissionUpdateOperation)}");
                    
                    var operation = _provider.GetRequiredService<PermissionUpdateOperation>();
                    using (MiniProfiler.Current.Step(nameof(PermissionUpdateOperation)))
                    {
                        result = await operation.ExecuteAsync(new PermissionUpdate
                            {
                                Id = id,
                                Name = request.Name,
                                DisplayName = request.DisplayName,
                                Description = request.Description
                            })
                            .ConfigureAwait(false);
                    }
                }
                else
                {
                    result = DomainError.PermissionError.InvalidId;
                }
                
                return _provider.GetService<IMapper<Result, UpdatePermissionReplay>>()
                    .Map(result);
            }
        }

        public override async Task<GetPermissionByIeReplay> GetPermissionById(GetPermissionByIdRequest request, ServerCallContext context)
        {
            using (MiniProfiler.Current.Step(nameof(GetPermissionById)))
            {
                Result result;
                if (Guid.TryParse(request.Id, out var id))
                {
                    _logger.LogInformation($"Going to execute {nameof(PermissionGetOperation)}");
                    
                    var operation = _provider.GetRequiredService<PermissionGetOperation>();
                    using (MiniProfiler.Current.Step(nameof(PermissionGetOperation)))
                    {
                        result = await operation.ExecuteAsync(new PermissionGetById
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

                return _provider.GetService<IMapper<Result, GetPermissionByIeReplay>>()
                    .Map(result);
            }
        }

        public override async Task GetPermissions(GetPermissionsRequest request, IServerStreamWriter<Permission> responseStream, ServerCallContext context)
        {
            _logger.LogInformation($"Going to execute {nameof(PermissionGetAllOperation)}");
                    
            var operation = _provider.GetRequiredService<PermissionGetAllOperation>();
            var result = await operation.ExecuteAsync(new PermissionGetAll())
                .ConfigureAwait(false);

            var mapper = _provider.GetRequiredService<IMapper<Domain.Common.Permission, Permission>>();
            if (result is OkResult<IAsyncEnumerable<Domain.Common.Permission>> ok)
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
    }
}