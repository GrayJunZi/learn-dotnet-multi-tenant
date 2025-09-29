using Microsoft.AspNetCore.Identity;

namespace PMS.Infrastructure.Identity.Models;

public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 是否活跃
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; }
    /// <summary>
    /// 刷新令牌过期时间
    /// </summary>
    public DateTime RefreshTokenExpiryTime { get; set; }
}
