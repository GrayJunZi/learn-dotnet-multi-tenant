using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;

namespace PMS.Infrastructure.Tenancy;

public class TenantDbContext(DbContextOptions<TenantDbContext> options)
    : EFCoreStoreDbContext<CompanyTenantInfo>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CompanyTenantInfo>().ToTable("Tenants", "MultiTenancy");
    }
}
