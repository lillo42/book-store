using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Web.IdentityServer4.Validators
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ILogger<ResourceOwnerPasswordValidator> _logger;
        private readonly IReadOnlyUserRepository _repository;
        private readonly ISystemClock _clock;
        private readonly IHashAlgorithm _hashAlgorithm;

        public ResourceOwnerPasswordValidator(IReadOnlyUserRepository repository,
            ISystemClock clock,
            IHashAlgorithm hashAlgorithm,
            ILogger<ResourceOwnerPasswordValidator> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hashAlgorithm = hashAlgorithm ?? throw new ArgumentNullException(nameof(hashAlgorithm));
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            _logger.LogInformation("Going to check user and password. [Mail: {mail}]", context.UserName);
            var user = await _repository.GetByMailAndPasswordAsync(context.UserName, _hashAlgorithm.ComputeHash(context.Password))
                .ConfigureAwait(false);
            if (user != default)
            {
                var claims = new List<Claim>();
                claims.AddRange(user.Roles.Select(x => new Claim("roles", x.Name)));
                claims.AddRange(user.Permissions.Select(x => new Claim("permissions", x.Name)));
                
                context.Result = new GrantValidationResult(user.Id.ToString(), 
                    OidcConstants.AuthenticationMethods.Password,
                    _clock.UtcNow.UtcDateTime,
                    claims.ToHashSet());
            }
            else
            {
                _logger.LogInformation("Mail/password invalid. [Mail: {mail}]", context.UserName);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "invalid username or password");
            }
        }
    }
}