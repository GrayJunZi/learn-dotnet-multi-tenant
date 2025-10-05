namespace PMS.Application.Features.Tenancy.Commands;

public class UpdateTenantSubscriptionRequest
{
    public string TenantId { get; set; }

    public DateTime ExpiryDate { get; set; }
}
