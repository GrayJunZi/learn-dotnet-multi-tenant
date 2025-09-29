using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using PMS.Domain.Entities;
using PMS.Infrastructure.Tenancy;

namespace PMS.Infrastructure.Contexts;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        IMultiTenantContextAccessor<CompanyTenantInfo> multiTenantContextAccessor,
        DbContextOptions options)
        : base(multiTenantContextAccessor, options)
    {

    }

    public DbSet<Company> Companies => Set<Company>();
}
