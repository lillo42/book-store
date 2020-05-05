using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityServer.Application.Operation.Resource;
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
    public class ResourceService : Resources.ResourcesBase
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<ResourceService> _logger;

        public ResourceService(IServiceProvider provider, ILogger<ResourceService> logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public override async Task<CreateResourceReplay> CreateResource(CreateResourceRequest request, ServerCallContext context)
        {
            using (MiniProfiler.Current.Step(nameof(CreateResource)))
            {
                _logger.LogInformation($"Going to execute {nameof(ResourceCreateOperation)}");
                var operation = _provider.GetRequiredService<ResourceCreateOperation>();
                var result = await operation.ExecuteAsync(new ResourceCreate
                {
                    Name = request.Name,
                    Description = request.Description,
                    DisplayName = request.DisplayName,
                    IsEnable = request.IsEnable
                }).ConfigureAwait(false);

                return _provider.GetService<IMapper<Result, CreateResourceReplay>>()
                    .Map(result);
            }
        }

        public override async Task<UpdateResourceReplay> UpdateResource(UpdateResourceRequest request, ServerCallContext context)
        {
            using (MiniProfiler.Current.Step(nameof(UpdateResource)))
            {
                Result result;
                if (Guid.TryParse(request.Id, out var id))
                {
                    _logger.LogInformation($"Going to execute {nameof(ResourceUpdateOperation)}");
                    
                    var operation = _provider.GetRequiredService<ResourceUpdateOperation>();
                    using (MiniProfiler.Current.Step(nameof(ResourceUpdateOperation)))
                    {
                        result = await operation.ExecuteAsync(new ResourceUpdate
                            {
                                Id = id,
                                Name = request.Name,
                                DisplayName = request.DisplayName,
                                Description = request.Description,
                                IsEnable = request.IsEnable
                            })
                            .ConfigureAwait(false);
                    }
                }
                else
                {
                    result = DomainError.ResourceError.InvalidId;
                }
                
                return _provider.GetService<IMapper<Result, UpdateResourceReplay>>()
                    .Map(result);
            }
        }

        public override async Task GetResources(GetResourcesRequest request, IServerStreamWriter<Resource> responseStream, ServerCallContext context)
        {
            _logger.LogInformation($"Going to execute {nameof(ResourceGetAllOperation)}");
                    
            var operation = _provider.GetRequiredService<ResourceGetAllOperation>();
            var result = await operation.ExecuteAsync(new ResourceGetAll())
                .ConfigureAwait(false);

            var mapper = _provider.GetRequiredService<IMapper<Domain.Common.Resource, Resource>>();
            if (result is OkResult<IAsyncEnumerable<Domain.Common.Resource>> ok)
            {
                await foreach (var resource in ok.Value
                    .WithCancellation(context.CancellationToken)
                    .ConfigureAwait(false))
                {
                    await responseStream.WriteAsync(mapper.Map(resource))
                        .ConfigureAwait(false);
                }
            }
        }

        public override async Task<GetResourceByIeReplay> GetResourceById(GetResourceByIdRequest request, ServerCallContext context)
        {
            using (MiniProfiler.Current.Step( nameof(GetResourceById)))
            {
                Result result;
                if (Guid.TryParse(request.Id, out var id))
                {
                    _logger.LogInformation($"Going to execute {nameof(ResourceGetOperation)}");
                    
                    var operation = _provider.GetRequiredService<ResourceGetOperation>();
                    
                    using (MiniProfiler.Current.Step(nameof(ResourceGetOperation)))
                    {
                        result = await operation.ExecuteAsync(new ResourceGetById
                            {
                                Id = id
                            })
                            .ConfigureAwait(false);
                    }
                }
                else
                {
                    result = DomainError.ResourceError.InvalidId;
                }

                return _provider.GetService<IMapper<Result, GetResourceByIeReplay>>()
                    .Map(result);
            }
        }
    }
}