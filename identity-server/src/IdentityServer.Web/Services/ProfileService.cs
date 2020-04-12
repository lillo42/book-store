using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Web.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ILogger<ProfileService> _logger;
        private readonly IReadOnlyUserRepository _repository;

        public ProfileService(IReadOnlyUserRepository repository, ILogger<ProfileService> logger)
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

                var user = await _repository.GetByIdAsync(Guid.Parse(subjectId))
                    .ConfigureAwait(false);
                
                if (user.Roles.Count == 0)
                {
                    context.AddRequestedClaims(user.Roles.Select(x => new Claim(JwtClaimTypes.Role, x.Name)));
                }
                
                if (user.Permissions.Count == 0)
                {
                    context.AddRequestedClaims(user.Permissions.Select(x => new Claim("permission", x.Name)));
                }
            }
            
            context.LogIssuedClaims(_logger);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("IsActive called from: {caller}", context.Caller);
            var subjectId = context.Subject.GetSubjectId();
            
            _logger.LogInformation("Going to check if '{userId}' user is enable.", subjectId);
            
            context.IsActive = await _repository.IsEnableAsync(Guid.Parse(subjectId))
                .ConfigureAwait(false);
        }
    }
}