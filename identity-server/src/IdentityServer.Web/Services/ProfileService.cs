using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Infrastructure.Repositories;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Web.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ILogger<ProfileService> _logger;
        private readonly IReadOnlyUsersRepository _repository;

        public ProfileService(IReadOnlyUsersRepository repository, ILogger<ProfileService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.LogProfileRequest(_logger);
            if (context.RequestedClaimTypes.Any())
            {
                var subjectId = context.Subject.GetSubjectId();
                _logger.LogInformation("Going to load '{userId}' profiling.", subjectId);

                var roles = await _repository.GetUsersRoles(Guid.Parse(subjectId))
                    .ConfigureAwait(false);
                if (roles != null)
                {
                    context.AddRequestedClaims(roles.Select(x => new Claim("roles", x.Name)));
                }
            }
            
            context.LogIssuedClaims(_logger);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("IsActive called from: {caller}", context.Caller);
            var subjectId = context.Subject.GetSubjectId();
            
            _logger.LogInformation("Going to check if '{userId}' user is enable.", subjectId);
            
            context.IsActive = await _repository.IsUserEnableAsync(Guid.Parse(subjectId))
                .ConfigureAwait(false);
        }
    }
}