using Microsoft.AspNetCore.Authorization;

namespace PMS.Infrastructure.Identity.Auth;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; set; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
