using System.Collections.ObjectModel;

namespace PMS.Infrastructure.Constants;

/// <summary>
/// 权限动作常量
/// </summary>
public static class ActionConstants
{
    /// <summary>
    /// 读取权限
    /// </summary>
    public const string Read = nameof(Read);
    /// <summary>
    /// 创建权限
    /// </summary>
    public const string Create = nameof(Create);
    /// <summary>
    /// 修改权限
    /// </summary>
    public const string Update = nameof(Update);
    /// <summary>
    /// 删除权限
    /// </summary>
    public const string Delete = nameof(Delete);
    /// <summary>
    /// 升级订阅权限
    /// </summary>
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

/// <summary>
/// 特性常量
/// </summary>
public static class FeatureConstants
{
    public const string Tenants = nameof(Tenants);
    public const string Users = nameof(Users);
    public const string Roles = nameof(Roles);
    public const string UserRoles = nameof(UserRoles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Companies = nameof(Companies);
}

/// <summary>
/// 公司权限
/// </summary>
/// <param name="Action"></param>
/// <param name="Feature"></param>
/// <param name="Description"></param>
/// <param name="IsBasic"></param>
/// <param name="IsRoot"></param>
public record CompanyPermission(string Action, string Feature, string Description, string Group, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Feature);
    public static string NameFor(string action, string feature) => $"Permission.{feature}.{action}";
}

public static class CompanyPermissions
{
    private static readonly CompanyPermission[] _allPermissions =
    [
        new CompanyPermission(ActionConstants.Create, FeatureConstants.Tenants, "Tenancy", "创建多租户", IsRoot: true),
        new CompanyPermission(ActionConstants.Read, FeatureConstants.Tenants, "读取多租户", "Tenancy", IsRoot: true),
        new CompanyPermission(ActionConstants.Update, FeatureConstants.Tenants, "修改多租户", "Tenancy", IsRoot: true),
        new CompanyPermission(ActionConstants.UpgradeSubscription, FeatureConstants.Tenants, "升级多租户订阅", "Tenancy", IsRoot: true),
        new CompanyPermission(ActionConstants.Delete, FeatureConstants.Tenants, "删除多租户", "Tenancy", IsRoot: true),

        new CompanyPermission(ActionConstants.Create, FeatureConstants.Users, "创建用户", "SystemAccess"),
        new CompanyPermission(ActionConstants.Read, FeatureConstants.Users, "读取用户", "SystemAccess"),
        new CompanyPermission(ActionConstants.Update, FeatureConstants.Users, "修改用户", "SystemAccess"),
        new CompanyPermission(ActionConstants.Delete, FeatureConstants.Users, "删除用户", "SystemAccess"),

        new CompanyPermission(ActionConstants.Read, FeatureConstants.UserRoles, "读取用户角色", "SystemAccess"),
        new CompanyPermission(ActionConstants.Update, FeatureConstants.UserRoles, "修改用户角色", "SystemAccess"),

        new CompanyPermission(ActionConstants.Create, FeatureConstants.Roles, "创建角色", "SystemAccess"),
        new CompanyPermission(ActionConstants.Read, FeatureConstants.Roles, "读取角色", "SystemAccess"),
        new CompanyPermission(ActionConstants.Update, FeatureConstants.Roles, "修改角色", "SystemAccess"),
        new CompanyPermission(ActionConstants.Delete, FeatureConstants.Roles, "删除角色", "SystemAccess"),

        new CompanyPermission(ActionConstants.Read, FeatureConstants.RoleClaims, "读取角色权限", "SystemAccess"),
        new CompanyPermission(ActionConstants.Update, FeatureConstants.RoleClaims, "修改角色权限", "SystemAccess"),

        new CompanyPermission(ActionConstants.Create, FeatureConstants.Companies, "创建公司", "Enterprise"),
        new CompanyPermission(ActionConstants.Read, FeatureConstants.Companies, "读取公司", "Enterprise", IsBasic: true),
        new CompanyPermission(ActionConstants.Update, FeatureConstants.Companies, "修改公司", "Enterprise"),
        new CompanyPermission(ActionConstants.Delete, FeatureConstants.Companies , "删除公司", "Enterprise"),
    ];

    public static IReadOnlyList<CompanyPermission> All { get; }
        = new ReadOnlyCollection<CompanyPermission>(_allPermissions);

    public static IReadOnlyList<CompanyPermission> Root { get; }
        = new ReadOnlyCollection<CompanyPermission>([.. _allPermissions.Where(x => x.IsRoot)]);

    public static IReadOnlyList<CompanyPermission> Admin { get; }
        = new ReadOnlyCollection<CompanyPermission>([.. _allPermissions.Where(x => !x.IsRoot)]);

    public static IReadOnlyList<CompanyPermission> Basic { get; }
        = new ReadOnlyCollection<CompanyPermission>([.. _allPermissions.Where(x => x.IsBasic)]);
}