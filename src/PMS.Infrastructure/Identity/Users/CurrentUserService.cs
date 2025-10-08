using PMS.Application.Exceptions;
using PMS.Application.Features.Identity.Users;
using System.Security.Claims;

namespace PMS.Infrastructure.Identity.Users;

public class CurrentUserService : ICurrentUserService
{
    private ClaimsPrincipal _claimsPrincipal;

    public string Name => _claimsPrincipal.GetName();

    public string GetUserId()
        => IsAuthenticated() ? _claimsPrincipal.GetUserId() : null;
    
    public  string GetUserEmail()
        => IsAuthenticated() ? _claimsPrincipal.GetEmail() : null;

    public string GetUserTenant()
        => IsAuthenticated() ? _claimsPrincipal?.GetTenant() : null;

    public bool IsAuthenticated()
        => _claimsPrincipal.Identity.IsAuthenticated;

    public bool IsInRole(string roleName)
        => _claimsPrincipal.IsInRole(roleName);

    public IEnumerable<Claim> GetClaims()
        => _claimsPrincipal.Claims;

    public bool SetCurrentUser(ClaimsPrincipal claimsPrincipal)
    {
        if (_claimsPrincipal is not null)
            throw new ConflictException(["无效操作。"]);

        _claimsPrincipal = claimsPrincipal;
        return true;
    }
}
