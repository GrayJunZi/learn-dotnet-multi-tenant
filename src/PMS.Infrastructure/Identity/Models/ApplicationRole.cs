using Microsoft.AspNetCore.Identity;

namespace PMS.Infrastructure.Identity.Models;

public class ApplicationRole : IdentityRole
{
    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; set; }
}