using Microsoft.AspNetCore.Mvc;
using PMS.Application.Features.Companies;
using PMS.Application.Features.Companies.Commands;
using PMS.Application.Features.Companies.Queries;
using PMS.Core.Constants;
using PMS.Infrastructure.Identity.Auth;

namespace PMS.WebApi.Controllers;

[Route("api/[controller]")]
public class CompaniesController : BaseApiController
{
    [HttpPost("add")]
    [ShouldHavePermission(ActionConstants.Create, FeatureConstants.Companies)]
    public async Task<IActionResult> CreateCompanyAsync([FromBody] CreateCompanyRequest createCompanyRequest)
    {
        var response = await Sender.Send(new CreateCompanyCommand { CreateCompany = createCompanyRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPut("update")]
    [ShouldHavePermission(ActionConstants.Update, FeatureConstants.Companies)]
    public async Task<IActionResult> UpdateCompanyAsync([FromBody] UpdateCompanyRequest updateCompanyRequest)
    {
        var response = await Sender.Send(new UpdateCompanyCommand { UpdateCompany = updateCompanyRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }

    [HttpDelete("{companyId}")]
    [ShouldHavePermission(ActionConstants.Delete, FeatureConstants.Companies)]
    public async Task<IActionResult> DeleteCompanyAsync(int companyId)
    {
        var response = await Sender.Send(new DeleteCompanyCommand { CompanyId = companyId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }

    [HttpGet("by-id/{companyId}")]
    [ShouldHavePermission(ActionConstants.Read, FeatureConstants.Companies)]
    public async Task<IActionResult> GetCompanyByIdAsync(int companyId)
    {
        var response = await Sender.Send(new GetCompanyByIdQuery { CompanyId = companyId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }

    [HttpGet("by-name/{companyName}")]
    [ShouldHavePermission(ActionConstants.Read, FeatureConstants.Companies)]
    public async Task<IActionResult> GetCompanyByIdAsync(string companyName)
    {
        var response = await Sender.Send(new GetCompanyByNameQuery { CompanyName = companyName });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }

    [HttpGet("all")]
    [ShouldHavePermission(ActionConstants.Read, FeatureConstants.Companies)]
    public async Task<IActionResult> GetCompaniesAsync(string companyName)
    {
        var response = await Sender.Send(new GetCompaniesQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return NotFound(response);
    }


}
