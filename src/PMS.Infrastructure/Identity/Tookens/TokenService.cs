using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PMS.Application;
using PMS.Application.Exceptions;
using PMS.Application.Features.Identity.Tokens;
using PMS.Core.Constants;
using PMS.Infrastructure.Identity.Models;
using PMS.Infrastructure.Tenancy;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PMS.Infrastructure.Identity.Tookens;

public class TokenService(
    IMultiTenantContextAccessor<CompanyTenantInfo> multiTenantContextAccessor,
    IOptions<JwtSettings> jwtSettinsOptions,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager
    ) : ITokenService
{
    public async Task<TokenResponse> LoginAsync(TokenRequest tokenRequest)
    {
        if (!multiTenantContextAccessor.MultiTenantContext.TenantInfo.IsActive)
            throw new UnauthorizedException(["租户未启用，请联系管理员。"]);

        if (multiTenantContextAccessor.MultiTenantContext.TenantInfo.Id is not TenancyConstants.Root.Id)
        {
            if (multiTenantContextAccessor.MultiTenantContext.TenantInfo.ValidUpTo < DateTime.UtcNow)
            {
                throw new UnauthorizedException(["租户已过期，请联系管理员。"]);
            }
        }

        var user = await userManager.FindByNameAsync(tokenRequest.UserName)
            ?? throw new UnauthorizedException(["身份认证失败。"]);

        if (!await userManager.CheckPasswordAsync(user, tokenRequest.Password))
            throw new UnauthorizedException(["用户名或密码错误。"]);

        if (!user.IsActive)
            throw new UnauthorizedException(["用户未启用，请联系管理员。"]);

        return await generateTokenAndUpdateUserAsync(user);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var userPricipal = getClaimsPrincipalFromExpiringToken(refreshTokenRequest.Jwt);
        var userEmail = userPricipal.GetEmail();

        var user = await userManager.FindByEmailAsync(userEmail)
            ?? throw new UnauthorizedException(["身份认证失败。"]);

        if (user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new UnauthorizedException(["Token 无效。"]);
        }

        return await generateTokenAndUpdateUserAsync(user);
    }

    private ClaimsPrincipal getClaimsPrincipalFromExpiringToken(string expiringToken)
    {
        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettinsOptions.Value.Secret))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var pricipal = tokenHandler.ValidateToken(expiringToken, tokenValidationParams, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException(["Token 生成失败。"]);
        }

        return pricipal;
    }

    private async Task<TokenResponse> generateTokenAndUpdateUserAsync(ApplicationUser user)
    {
        var jwt = await generateToken(user);

        user.RefreshToken = generateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtSettinsOptions.Value.RefreshTokenExpiryTimeInDays);

        await userManager.UpdateAsync(user);

        return new TokenResponse
        {
            Jwt = jwt,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryDate = user.RefreshTokenExpiryTime,
        };
    }

    private async Task<string> generateToken(ApplicationUser user)
    {
        return generateEncryptedToken(generateSigningCredentials(), await getUserClaims(user));
    }

    private string generateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var jwtSecurityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettinsOptions.Value.TokenExpiryTimeInMinutes),
            signingCredentials: signingCredentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(jwtSecurityToken);
    }

    private SigningCredentials generateSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(jwtSettinsOptions.Value.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private async Task<IEnumerable<Claim>> getUserClaims(ApplicationUser user)
    {
        var userClaims = await userManager.GetClaimsAsync(user);
        var userRoles = await userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();

        foreach (var userRole in userRoles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, userRole));

            var role = await roleManager.FindByNameAsync(userRole);
            permissionClaims.AddRange(await roleManager.GetClaimsAsync(role));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier,user.Id),
            new(ClaimTypes.Name,user.Name),
            new(ClaimTypes.Email,user.Email),
            new(ClaimTypes.Surname,user.Name),
            new(ClaimConstants.Tenant,multiTenantContextAccessor.MultiTenantContext.TenantInfo.Id),
            new(ClaimTypes.MobilePhone,user.PhoneNumber??string.Empty),
        }
        .Union(userClaims)
        .Union(roleClaims)
        .Union(permissionClaims);

        return claims;
    }

    private string generateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }
}
