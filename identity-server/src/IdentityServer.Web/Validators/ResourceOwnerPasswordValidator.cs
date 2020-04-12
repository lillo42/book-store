using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Infrastructure.Abstractions.Repositories;
using IdentityServer.Infrastructure.Abstractions.Repositories.ReadOnly;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Web.Validators
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ILogger<ResourceOwnerPasswordValidator> _logger;
        private readonly IReadOnlyUserRepository _repository;
        private readonly ISystemClock _clock;

        public ResourceOwnerPasswordValidator(IReadOnlyUserRepository repository,
            ISystemClock clock,
            ILogger<ResourceOwnerPasswordValidator> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            _logger.LogInformation("Going to check user and password. [Mail: {mail}]", context.UserName);
            var password = ComputeHash(context.Password);
            var user = await _repository.GetByMailAndPasswordAsync(context.UserName, password).ConfigureAwait(false);
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

        private static string ComputeHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }

            using var hashAlgorithm = SHA256.Create();
            var data = Encoding.UTF8.GetBytes(password);
            var hash = hashAlgorithm.ComputeHash(data);
            
            var sb = new StringBuilder();
            foreach (var @byte in hash)
            {
                sb.Append(@byte.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}