namespace PMS.Application.Features.Tenancy;

public class TenantResponse
{
    /// <summary>
    /// 多租户标识符
    /// </summary>
    public string Identifier { get; set; }
    /// <summary>
    /// 多租户名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 多租户连接字符串
    /// </summary>
    public string ConnectionString { get; set; }
    /// <summary>
    /// 多租户管理员邮箱
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// 多租户管理员联系人
    /// </summary>
    public string Contact { get; set; }
    /// <summary>
    /// 有效期
    /// </summary>
    public DateTime ValidUpTo { get; set; }
    /// <summary>
    /// 是否活跃
    /// </summary>
    public bool IsActive { get; set; }

}
