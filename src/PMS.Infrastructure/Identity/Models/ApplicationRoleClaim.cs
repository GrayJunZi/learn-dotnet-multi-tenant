using Microsoft.AspNetCore.Identity;

namespace PMS.Infrastructure.Identity.Models;

public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 组
    /// </summary>
    public string Group { get; set; }
}
