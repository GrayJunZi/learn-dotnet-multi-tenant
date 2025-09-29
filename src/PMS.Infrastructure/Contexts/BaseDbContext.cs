using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PMS.Infrastructure.Identity.Models;
using PMS.Infrastructure.Tenancy;
using System.Reflection;

namespace PMS.Infrastructure.Contexts;

public abstract class BaseDbContext
    : MultiTenantIdentityDbContext<ApplicationUser, ApplicationRole, string,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken>
{
    private new CompanyTenantInfo TenantInfo { get; set; }

    protected BaseDbContext(IMultiTenantContextAccessor<CompanyTenantInfo> multiTenantContextAccessor, DbContextOptions options)
        : base(multiTenantContextAccessor, options)
    {
        TenantInfo = multiTenantContextAccessor.MultiTenantContext.TenantInfo;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (string.IsNullOrEmpty(TenantInfo?.ConnectionString))
        {
            return;
        }

        optionsBuilder.UseNpgsql(TenantInfo.ConnectionString, options =>
        {
            options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
