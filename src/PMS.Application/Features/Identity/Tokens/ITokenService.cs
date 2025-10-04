namespace PMS.Application.Features.Identity.Tokens;

public interface ITokenService
{
    Task<TokenResponse> LoginAsync(TokenRequest tokenRequest);

    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
}