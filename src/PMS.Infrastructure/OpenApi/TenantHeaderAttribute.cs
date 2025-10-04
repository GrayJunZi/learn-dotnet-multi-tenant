using PMS.Infrastructure.Tenancy;

namespace PMS.Infrastructure.OpenApi;

public class TenantHeaderAttribute()
    : ScalarHeaderAttribute(
        HeaderName: TenancyConstants.TenantIdName,
        Description: "输入你的租户名称来访问API。",
        DefaultValue: string.Empty,
        IsRequired: true)
{

}