using Microsoft.AspNetCore.Authorization;
using PMS.Core.Constants;

namespace PMS.WebApp.Infrastructure.Services.Auth;

public class ShouldHavePermissionAttribute : AuthorizeAttribute
{
    public ShouldHavePermissionAttribute(string action, string feature)
    {
        Policy = CompanyPermission.NameFor(action, feature);
    }
}
