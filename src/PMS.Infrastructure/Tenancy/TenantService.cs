using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using PMS.Application.Features.Tenancy;
using PMS.Application.Features.Tenancy.Commands;
using PMS.Infrastructure.Contexts;

namespace PMS.Infrastructure.Tenancy;

public class TenantService(
    IMultiTenantStore<CompanyTenantInfo> multiTenantStore,
    ApplicationDbSeeder applicationDbSeeder,
    IServiceProvider serviceProvider
    ) : ITenantService
{
    public async Task<string> CreateTenantAsync(CreateTenantRequest createTenantRequest, CancellationToken cancellationToken)
    {
        var tenant = new CompanyTenantInfo
        {
            Id = createTenantRequest.Identifier,
            Identifier = createTenantRequest.Identifier,
            Name = createTenantRequest.Name,
            IsActive = createTenantRequest.IsActive,
            ConnectionString = createTenantRequest.ConnectionString,
            Email = createTenantRequest.Email,
            ValidUpTo = createTenantRequest.ValidUpTo,
        };

        await multiTenantStore.TryAddAsync(tenant);

        using var scope = serviceProvider.CreateScope();
        serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
            .MultiTenantContext = new MultiTenantContext<CompanyTenantInfo>
            {
                TenantInfo = tenant,
            };

        await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);

        return tenant.Identifier;
    }

    public async Task<string> ActivateAsync(string id)
    {
        var tenant = await multiTenantStore.TryGetAsync(id);
        if (tenant is null)
            return null;

        tenant.IsActive = true;
        await multiTenantStore.TryUpdateAsync(tenant);
        return tenant.Identifier;
    }

    public async Task<string> DeactivateAsync(string id)
    {
        var tenant = await multiTenantStore.TryGetAsync(id);
        if (tenant is null)
            return null;

        tenant.IsActive = false;
        await multiTenantStore.TryUpdateAsync(tenant);
        return tenant.Identifier;
    }

    public async Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscriptionRequest)
    {
        var tenant = await multiTenantStore.TryGetAsync(updateTenantSubscriptionRequest.TenantId);
        if (tenant is null)
            return null;

        tenant.ValidUpTo = updateTenantSubscriptionRequest.ExpiryDate;
        await multiTenantStore.TryUpdateAsync(tenant);
        return tenant.Identifier;
    }

    public async Task<List<TenantResponse>> GetTenantsAsync()
    {
        var tenants = await multiTenantStore.GetAllAsync();
        return tenants.Adapt<List<TenantResponse>>();
    }

    public async Task<TenantResponse> GetTenantByIdAsync(string id)
    {
        var tenant = await multiTenantStore.TryGetAsync(id);

        return tenant.Adapt<TenantResponse>();
    }
}
