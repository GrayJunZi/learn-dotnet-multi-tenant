using System.Collections.ObjectModel;

namespace PMS.Core.Constants;

/// <summary>
/// 角色常量
/// </summary>
public static class RoleConstants
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);

    /// <summary>
    /// 默认角色
    /// </summary>
    public static IReadOnlyList<string> DefaultRoles { get; }
        = new ReadOnlyCollection<string>([Admin, Basic]);

    public static bool IsDefaultRole(string roleName)
        => DefaultRoles.Contains(roleName);
}
