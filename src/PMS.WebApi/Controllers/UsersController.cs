using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.Application.Features.Identity.Users;
using PMS.Application.Features.Identity.Users.Commands;
using PMS.Application.Features.Identity.Users.Queries;
using PMS.Core.Constants;
using PMS.Infrastructure.Identity.Auth;

namespace PMS.WebApi.Controllers;

[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    [HttpPost("register")]
    [ShouldHavePermission(ActionConstants.Create, FeatureConstants.Users)]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest createUserRequest)
    {
        var response = await Sender.Send(new CreateUserCommand { CreateUser = createUserRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPut("update")]
    [ShouldHavePermission(ActionConstants.Update, FeatureConstants.Users)]
    public async Task<IActionResult> UpdateUserDetailsAsync([FromBody] UpdateUserRequest updateUserRequest)
    {
        var response = await Sender.Send(new UpdateUserCommand { UpdateUser = updateUserRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpPut("update-status")]
    [ShouldHavePermission(ActionConstants.Update, FeatureConstants.Users)]
    public async Task<IActionResult> ChangeUserStatusAsync([FromBody] ChangeUserStatusRequest changeUserStatusRequest)
    {
        var response = await Sender.Send(new ChangeUserStatusCommand { ChangeUserStatus = changeUserStatusRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpPut("update-roles/{userId}")]
    [ShouldHavePermission(ActionConstants.Update, FeatureConstants.UserRoles)]
    public async Task<IActionResult> UpdateUserRolesAsync([FromBody] UserRolesRequest userRolesRequest, string userId)
    {
        var response = await Sender.Send(new UpdateUserRolesCommand { UserRoles = userRolesRequest, UserId = userId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpDelete("delete/{userId}")]
    [ShouldHavePermission(ActionConstants.Delete, FeatureConstants.Users)]
    public async Task<IActionResult> DeleteUserAsync(string userId)
    {
        var response = await Sender.Send(new DeleteUserCommand { UserId = userId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpGet("all")]
    [ShouldHavePermission(ActionConstants.Read, FeatureConstants.Users)]
    public async Task<IActionResult> GetUsersAsync()
    {
        var response = await Sender.Send(new GetAllUsersQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpGet("{userId}")]
    [ShouldHavePermission(ActionConstants.Read, FeatureConstants.Users)]
    public async Task<IActionResult> GetUserByIdAsync(string userId)
    {
        var response = await Sender.Send(new GetUserByIdQuery { UserId = userId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpGet("permissions/{userId}")]
    [ShouldHavePermission(ActionConstants.Read, FeatureConstants.RoleClaims)]
    public async Task<IActionResult> GetUserPermissionsAsync(string userId)
    {
        var response = await Sender.Send(new GetUserPermissionsQuery { UserId = userId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpGet("user-roles/{userId}")]
    [ShouldHavePermission(ActionConstants.Read, FeatureConstants.UserRoles)]
    public async Task<IActionResult> GetUserRolesAsync(string userId)
    {
        var response = await Sender.Send(new GetUserRolesQuery { UserId = userId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [HttpPut("change-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangeUserPasswordAsync([FromBody] ChangePasswordRequest changePasswordRequest)
    {
        var response = await Sender.Send(new ChangeUserPasswordCommand { ChangePassword = changePasswordRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
}