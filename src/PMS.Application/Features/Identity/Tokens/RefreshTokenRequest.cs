namespace PMS.Application.Features.Identity.Tokens;

public class RefreshTokenRequest
{
    public string Jwt { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpirayDate { get; set; }
}