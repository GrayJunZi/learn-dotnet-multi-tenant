using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using PMS.Infrastructure.Constants;

namespace PMS.Infrastructure.Identity.Auth;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider FallbackProvider { get; }
        = new DefaultAuthorizationPolicyProvider(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return FallbackProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
    {
        return Task.FromResult<AuthorizationPolicy>(null);
    }

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if(policyName?.StartsWith(ClaimConstants.Permission,StringComparison.OrdinalIgnoreCase) is true)
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
            return Task.FromResult(policy);
        }

        return FallbackProvider.GetPolicyAsync(policyName);
    }
}
