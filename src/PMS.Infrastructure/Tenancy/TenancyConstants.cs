namespace PMS.Infrastructure.Tenancy;

public static class TenancyConstants
{
    public const string TenantIdName = "tenant";

    public const string DefaultPassword = "P@ssw0rd@123";
    public const string Name = "PMS";

    public static class Root
    {
        public const string Id = "root";
        public const string Name = "Root";
        public const string Email = "admin.root@pms.com";
    }
}
