using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using IdentityModel;
using IdentityServer.Infrastructure.Repositories;
using IdentityServer.Web.Modules;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Web.Validators
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ILogger<ResourceOwnerPasswordValidator> _logger;
        private readonly IReadOnlyUsersRepository _repository;
        private readonly ISystemClock _clock;

        public ResourceOwnerPasswordValidator(IReadOnlyUsersRepository repository,
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
            var userId = await _repository.GetByEmailAsync(context.UserName, password).ConfigureAwait(false);
            if (userId != default)
            {
                
                context.Result = new GrantValidationResult(userId.ToString(), 
                    OidcConstants.AuthenticationMethods.Password,
                    _clock.UtcNow.UtcDateTime,
                    (await _repository.GetUsersRoles(userId)
                        .ConfigureAwait(false))
                    .Select(x => new Claim("roles", x.Name)).ToHashSet());
            }
            else
            {
                _logger.LogInformation("Mail/password invalid. [Mail: {mail}]", context.UserName);
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "invalid username or password");
            }
        }
        
        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<RepositoryModule>();
        }

        private static string ComputeHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }

            using var hashAlgorithm = SHA1.Create();
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