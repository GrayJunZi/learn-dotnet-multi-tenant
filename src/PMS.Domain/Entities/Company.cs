namespace PMS.Domain.Entities;

/// <summary>
/// 公司
/// </summary>
public class Company
{
    /// <summary>
    /// 公司Id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 公司名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 建立日期
    /// </summary>
    public DateTime EstablishedDate { get; set; }
}
