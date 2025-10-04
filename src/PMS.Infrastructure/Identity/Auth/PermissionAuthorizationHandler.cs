using Microsoft.AspNetCore.Authorization;
using PMS.Infrastructure.Constants;

namespace PMS.Infrastructure.Identity.Auth;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissions = context.User.Claims
            .Where(x => x.Type == ClaimConstants.Permission && x.Value == requirement.Permission);

        if (permissions.Any())
        {
            context.Succeed(requirement);
            await Task.CompletedTask;
        }
    }
}
