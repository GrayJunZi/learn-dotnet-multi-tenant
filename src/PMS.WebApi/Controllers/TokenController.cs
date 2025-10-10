using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.Application.Features.Identity.Tokens;
using PMS.Application.Features.Identity.Tokens.Queries;
using PMS.Core.Constants;
using PMS.Infrastructure.Identity.Auth;
using PMS.Infrastructure.OpenApi;

namespace PMS.WebApi.Controllers;

[Route("api/[controller]")]
public class TokenController : BaseApiController
{
    /// <summary>
    /// 登录
    /// </summary>
    /// <returns></returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [TenantHeader]
    public async Task<IActionResult> Login([FromBody] TokenRequest tokenRequest)
    {
        var response = await Sender.Send(new GetTokenQuery { TokenRequest = tokenRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    /// <summary>
    /// 生成新Token
    /// </summary>
    /// <param name="refreshTokenRequest"></param>
    /// <returns></returns>
    [HttpPost("refresh-token")]
    [TenantHeader]
    [ShouldHavePermission(ActionConstants.RefreshToken,FeatureConstants.Tokens)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var response = await Sender.Send(new GetRefreshTokenQuery { RefreshTokenRequest = refreshTokenRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}
