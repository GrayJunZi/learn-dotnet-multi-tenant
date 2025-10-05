using PMS.Application.Features.Tenancy.Commands;

namespace PMS.Application.Features.Tenancy;

public interface ITenantService
{
    Task<string> CreateTenantAsync(CreateTenantRequest createTenantRequest, CancellationToken cancellationToken);

    Task<string> ActivateAsync(string id);

    Task<string> DeactivateAsync(string id);

    Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest updateTenantSubscriptionRequest);

    Task<List<TenantResponse>> GetTenantsAsync();

    Task<TenantResponse> GetTenantByIdAsync(string id);
}
