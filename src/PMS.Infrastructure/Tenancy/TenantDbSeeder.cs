using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PMS.Infrastructure.Contexts;

namespace PMS.Infrastructure.Tenancy;

public class TenantDbSeeder(
    IServiceProvider serviceProvider,
    TenantDbContext tenantDbContext) : ITenantDbSeeder
{
    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        await initializeDatabaseWithTenantAsync(cancellationToken);

        foreach (var tenant in await tenantDbContext.TenantInfo.ToListAsync(cancellationToken))
        {
            await initializeApplicationDbForTenantAsync(tenant, cancellationToken);
        }
    }

    private async Task initializeDatabaseWithTenantAsync(CancellationToken cancellationToken)
    {
        if (await tenantDbContext.TenantInfo.FindAsync([TenancyConstants.Root.Id], cancellationToken) is null)
        {
            var rootTenant = new CompanyTenantInfo
            {
                Id = TenancyConstants.Root.Id,
                Identifier = TenancyConstants.Root.Id,
                Name = TenancyConstants.Root.Name,
                Email = TenancyConstants.Root.Email,
                IsActive = true,
                ValidUpTo = DateTime.UtcNow.AddYears(2),
            };

            await tenantDbContext.TenantInfo.AddAsync(rootTenant, cancellationToken);
            await tenantDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task initializeApplicationDbForTenantAsync(CompanyTenantInfo tenant, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        serviceProvider.GetRequiredService<IMultiTenantContextSetter>().MultiTenantContext = new MultiTenantContext<CompanyTenantInfo>()
        {
            TenantInfo = tenant,
        };

        await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);
    }
}