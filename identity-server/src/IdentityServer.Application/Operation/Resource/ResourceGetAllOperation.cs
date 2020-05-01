using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Application.Request.Resource;
using IdentityServer.Domain.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Application.Operation.Resource
{
    public class ResourceGetAllOperation : IOperation<ResourceGetAll>
    {
        private readonly ILogger<ResourceGetAllOperation> _logger;
        private readonly IReadOnlyResourceRepository _permissionRepository;

        public ResourceGetAllOperation(IReadOnlyResourceRepository permissionRepository,
            ILogger<ResourceGetAllOperation> logger)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> ExecuteAsync(ResourceGetAll request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Going to get all resource");
            try
            {
                return Result.Ok(_permissionRepository.GetAllAsync(cancellationToken));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to execute get all permission");
                return Result.Fail(e);
            }
        }
    }
}