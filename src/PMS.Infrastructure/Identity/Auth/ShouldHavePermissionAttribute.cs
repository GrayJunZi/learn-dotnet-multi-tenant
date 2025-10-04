using Microsoft.AspNetCore.Authorization;
using PMS.Infrastructure.Constants;

namespace PMS.Infrastructure.Identity.Auth;

public class ShouldHavePermissionAttribute : AuthorizeAttribute
{
    public ShouldHavePermissionAttribute(string action, string feature)
    {
        Policy = CompanyPermission.NameFor(action, feature);
    }
}
