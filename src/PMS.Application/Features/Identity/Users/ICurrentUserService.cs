using System.Security.Claims;

namespace PMS.Application.Features.Identity.Users;

public interface ICurrentUserService
{
    string Name { get; }
    string GetUserId();
    string GetUserEmail();
    string GetUserTenant();
    bool IsAuthenticated();
    bool IsInRole(string roleName);
    IEnumerable<Claim> GetClaims();
    bool SetCurrentUser(ClaimsPrincipal claimsPrincipal);
}
