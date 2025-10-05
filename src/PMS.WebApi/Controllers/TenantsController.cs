using MediatR;
using Microsoft.AspNetCore.Mvc;
using PMS.Application.Features.Tenancy;
using PMS.Application.Features.Tenancy.Commands;
using PMS.Application.Features.Tenancy.Queryies;
using PMS.Infrastructure.Constants;
using PMS.Infrastructure.Identity.Auth;

namespace PMS.WebApi.Controllers;

[Route("api/[controller]")]
public class TenantsController : BaseApiController
{
    [HttpPost("add")]
    [ShouldHavePermission(ActionConstants.Create, FeatureConstants.Tenants)]
    public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantRequest createTenantRequest)
    {
        var response = await Sender.Send(new CreateTenantCommand { CreateTenant = createTenantRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPut("{tenantId}/activate")]
    [ShouldHavePermission(ActionConstants.Update, FeatureConstants.Tenants)]
    public async Task<IActionResult> ActivateTenantAsync(string tenantId)
    {
        var response = await Sender.Send(new ActivateTenantCommand { TenantId = tenantId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPut("{tenantId}/deactivate")]
    [ShouldHavePermission(ActionConstants.Update, FeatureConstants.Tenants)]
    public async Task<IActionResult> DeactivateTenantAsync(string tenantId)
    {
        var response = await Sender.Send(new DeactivateTenantCommand { TenantId = tenantId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPut("upgrade")]
    [ShouldHavePermission(ActionConstants.UpgradeSubscription, FeatureConstants.Tenants)]
    public async Task<IActionResult> UpgradeTenantSubscriptionAsync([FromBody] UpdateTenantSubscriptionRequest updateTenantSubscriptionRequest)
    {
        var response = await Sender.Send(new UpdateTenantSubscriptionCommand { UpdateTenantSubscription = updateTenantSubscriptionRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpGet("{tenantId}")]
    [ShouldHavePermission(ActionConstants.Read, FeatureConstants.Tenants)]
    public async Task<IActionResult> GetTenantByIdAsync(string tenantId)
    {
        var response = await Sender.Send(new GetTenantByIdQuery { TenantId = tenantId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpGet("all")]
    [ShouldHavePermission(ActionConstants.Read, FeatureConstants.Tenants)]
    public async Task<IActionResult> GetTenantsAsync()
    {
        var response = await Sender.Send(new GetTenantsQuery { });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
