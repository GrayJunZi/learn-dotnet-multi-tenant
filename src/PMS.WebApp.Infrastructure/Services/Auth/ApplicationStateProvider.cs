using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PMS.Core.Constants;
using PMS.WebApp.Infrastructure.Constants;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace PMS.WebApp.Infrastructure.Services.Auth;

public class ApplicationStateProvider(
    ILocalStorageService localStorageService,
    HttpClient httpClient) : AuthenticationStateProvider
{
    public ClaimsPrincipal AuthenticationStateUser { get; set; }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await localStorageService.GetItemAsStringAsync(StorageConstants.AuthToken);
        if (string.IsNullOrEmpty(savedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        httpClient .DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);

        var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(getClaimsFromJwt(savedToken), "jwt")));
        AuthenticationStateUser = state.User;
        return state;
    }

    public void MarkUserAuthenticated(string username)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Email, username)], "apiauth"));

        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

        NotifyAuthenticationStateChanged(authState);
    }

    public void MarkUserLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());

        var authState = Task.FromResult(new AuthenticationState(anonymousUser));

        NotifyAuthenticationStateChanged(authState);
    }

    public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
    {
        var state = await GetAuthenticationStateAsync();

        return state.User;
    }

    private IEnumerable<Claim> getClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var bytes = parseBase64WithoutPadding(payload);

        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(bytes);
        if (keyValuePairs != null)
        {
            if (keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles) && roles != null)
            {
                if (roles.ToString().Trim().StartsWith('['))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());
                    claims.AddRange(parsedRoles.Select(roleName => new Claim(ClaimTypes.Role, roleName)));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            if (keyValuePairs.TryGetValue(ClaimConstants.Permission, out var permissions) && permissions != null)
            {
                if (permissions.ToString().Trim().StartsWith('['))
                {
                    var parsedPermissions = JsonSerializer.Deserialize<string[]>(permissions.ToString());
                    claims.AddRange(parsedPermissions.Select(permission => new Claim(ClaimConstants.Permission, permission)));
                }
                else
                {
                    claims.Add(new Claim(ClaimConstants.Permission, permissions.ToString()));
                }
            }

            claims.AddRange(keyValuePairs.Select(x => new Claim(x.Key, x.Value.ToString())));
        }

        return claims;
    }

    private byte[] parseBase64WithoutPadding(string base64Payload)
    {
        switch (base64Payload.Length % 4)
        {
            case 2:
                base64Payload += "==";
                break;
            case 3:
                base64Payload += "=";
                break;
            default:
                break;
        }

        return Convert.FromBase64String(base64Payload);
    }
}
