using Finbuckle.MultiTenant.Abstractions;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PMS.Application.Exceptions;
using PMS.Application.Features.Identity.Users;
using PMS.Infrastructure.Constants;
using PMS.Infrastructure.Contexts;
using PMS.Infrastructure.Identity.Models;
using PMS.Infrastructure.Tenancy;

namespace PMS.Infrastructure.Identity.Users;

public class UserService(
    IMultiTenantContextAccessor<CompanyTenantInfo> multiTenantContextAccessor,
    ApplicationDbContext applicationDbContext,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : IUserService
{
    public async Task<string> CreateAsync(CreateUserRequest createUserRequest)
    {
        if (createUserRequest.Password != createUserRequest.ConfirmPassword)
        {
            throw new ConflictException(["密码不一致。"]);
        }

        if (await IsEmailTakenAsync(createUserRequest.Email))
        {
            throw new ConflictException(["邮箱已存在。"]);
        }

        var user = new ApplicationUser
        {
            Name = createUserRequest.Name,
            Email = createUserRequest.Email,
            PhoneNumber = createUserRequest.PhoneNumber,
            IsActive = createUserRequest.IsActive,
            UserName = createUserRequest.Email,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(user, createUserRequest.Password);
        if (!result.Succeeded)
        {
            throw new IdentityException(result.GetIdentityResultErrorDescrptions());
        }

        return user.Id;
    }

    public async Task<string> UpdateAsync(UpdateUserRequest updateUserRequest)
    {
        var user = await getUserAsync(updateUserRequest.Id);

        user.Name = updateUserRequest.Name;
        user.PhoneNumber = updateUserRequest.PhoneNumber;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new IdentityException(result.GetIdentityResultErrorDescrptions());
        }

        return user.Id;
    }

    public async Task<string> DeleteAsync(string id)
    {
        var user = await getUserAsync(id);

        if (user.Email == TenancyConstants.Root.Email)
        {
            throw new ConflictException(["不允许移除根租户下的管理员角色用户。"]);
        }

        applicationDbContext.Users.Remove(user);
        await applicationDbContext.SaveChangesAsync();

        return id;
    }
    public async Task<string> ActivateOrDeactivateAsync(ChangeUserStatusRequest changeUserStatusRequest)
    {
        var user = await getUserAsync(changeUserStatusRequest.UserId);
        user.IsActive = changeUserStatusRequest.Activation;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new IdentityException(result.GetIdentityResultErrorDescrptions());
        }

        return user.Id;
    }
    public async Task<string> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest)
    {
        var user = await getUserAsync(changePasswordRequest.UserId);

        if (changePasswordRequest.NewPassword != changePasswordRequest.ConfirmPassword)
        {
            throw new ConflictException(["密码不一致。"]);
        }

        var result = await userManager.ChangePasswordAsync(user, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);

        if (!result.Succeeded)
        {
            throw new IdentityException(result.GetIdentityResultErrorDescrptions());
        }

        return user.Id;
    }

    public async Task<string> AssignRolesAsync(string userId, UserRolesRequest userRolesRequest)
    {
        var user = await getUserAsync(userId);

        if (await userManager.IsInRoleAsync(user, RoleConstants.Admin)
            && userRolesRequest.UserRoles.Any(x => !x.IsAssigned && x.Name == RoleConstants.Admin))
        {
            var adminUserCount = (await userManager.GetUsersInRoleAsync(RoleConstants.Admin)).Count;
            if (user.Email == TenancyConstants.Root.Email)
            {
                if (multiTenantContextAccessor.MultiTenantContext.TenantInfo.Id == TenancyConstants.Root.Id)
                {
                    throw new ConflictException(["不允许移除根租户下的管理员角色用户。"]);
                }
            }
            else if (adminUserCount <= 2)
            {
                throw new ConflictException(["不允许移除，租户至少应有两位管理员用户。"]);
            }
        }

        foreach (var userRole in userRolesRequest.UserRoles)
        {
            if (userRole.IsAssigned)
            {
                if (!await userManager.IsInRoleAsync(user, userRole.Name))
                {
                    await userManager.AddToRoleAsync(user, userRole.Name);
                }
            }
            else
            {
                await userManager.RemoveFromRoleAsync(user, userRole.Name);
            }
        }

        return user.Id;
    }

    public async Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await userManager.Users.ToListAsync(cancellationToken);

        return users.Adapt<List<UserResponse>>();
    }

    public async Task<UserResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var user = await getUserAsync(id);

        return user.Adapt<UserResponse>();
    }

    public async Task<List<UserRoleResponse>> GetUserRolesAsync(string id, CancellationToken cancellationToken)
    {
        var user = await getUserAsync(id);

        var roles = await roleManager.Roles.ToListAsync(cancellationToken);

        var userRoles = new List<UserRoleResponse>();
        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleResponse
            {
                RoleId = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsAssigned = await userManager.IsInRoleAsync(user, role.Name)
            });
        }

        return userRoles;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await userManager.FindByEmailAsync(email) is not null;
    }

    public async Task<List<string>> GetUserPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        var user = await getUserAsync(id);

        var userRolesNames = await userManager.GetRolesAsync(user);

        var roles = await roleManager.Roles.Where(x => userRolesNames.Contains(x.Name))
             .ToListAsync();

        var permissions = await applicationDbContext.RoleClaims.Where(x => roles.Any(y => y.Id == x.RoleId) && x.ClaimType == ClaimConstants.Permission)
            .Select(x => x.ClaimValue)
            .Distinct()
            .ToListAsync(cancellationToken);

        return permissions;
    }

    public async Task<bool> IsPermissionAssignedAsync(string id, string permission, CancellationToken cancellationToken)
    {
        return (await GetUserPermissionsAsync(id, cancellationToken)).Contains(permission);
    }

    private async Task<ApplicationUser> getUserAsync(string userId)
    {
        return await userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException(["用户不存在。"]);
    }
}
