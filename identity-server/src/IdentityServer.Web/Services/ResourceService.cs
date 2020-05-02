using System;
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
            using (MiniProfiler.Current.Step("CreateResource"))
            {
                _logger.LogInformation("Going to execute ResourceCreateOperation");
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

        public override async Task<GetResourceByIeReplay> GetResourceById(GetResourceByIdRequest request, ServerCallContext context)
        {
            using (MiniProfiler.Current.Step("GetResourceById"))
            {
                Result result;
                if (Guid.TryParse(request.Id, out var id))
                {
                    var operation = _provider.GetRequiredService<ResourceGetOperation>();
                    result = await operation.ExecuteAsync(new ResourceGetById
                        {
                            Id = id
                        })
                        .ConfigureAwait(false);
                }
                else
                {
                    result = DomainError.ResourceError.InvalidId;
                }

                _logger.LogInformation("Going to execute ResourceCreateOperation");

                return _provider.GetService<IMapper<Result, GetResourceByIeReplay>>()
                    .Map(result);
            }
        }
    }
}