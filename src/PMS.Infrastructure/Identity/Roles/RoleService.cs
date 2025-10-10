using Finbuckle.MultiTenant.Abstractions;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PMS.Application.Exceptions;
using PMS.Application.Features.Identity.Roles;
using PMS.Core.Constants;
using PMS.Infrastructure.Contexts;
using PMS.Infrastructure.Identity.Models;
using PMS.Infrastructure.Tenancy;

namespace PMS.Infrastructure.Identity.Roles;

public class RoleService(
    IMultiTenantContextAccessor<CompanyTenantInfo> multiTenantContextAccessor,
    ApplicationDbContext applicationDbContext,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : IRoleService
{
    public async Task<string> CreateAsync(CreateRoleRequest createRoleRequest)
    {
        var role = new ApplicationRole
        {
            Name = createRoleRequest.Name,
            Description = createRoleRequest.Description,
        };

        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new IdentityException(result.GetIdentityResultErrorDescrptions());
        }

        return role.Name;
    }

    public async Task<string> UpdateAsync(UpdateRoleRequest updateRoleRequest)
    {
        var role = await roleManager.FindByIdAsync(updateRoleRequest.Id)
            ?? throw new NotFoundException(["角色不存在。"]);

        if (RoleConstants.IsDefaultRole(role.Name))
        {
            throw new ConflictException([$"不允许修改 '{role.Name}' 角色。"]);
        }

        role.Name = updateRoleRequest.Name;
        role.Description = updateRoleRequest.Description;
        role.NormalizedName = updateRoleRequest.Name.ToUpperInvariant();

        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            throw new IdentityException(result.GetIdentityResultErrorDescrptions());
        }

        return role.Name;
    }

    public async Task<string> DeleteAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id)
            ?? throw new NotFoundException(["角色不存在。"]);

        if (RoleConstants.IsDefaultRole(role.Name))
        {
            throw new ConflictException([$"不允许删除 '{role.Name}' 角色。"]);
        }

        if ((await userManager.GetUsersInRoleAsync(role.Name))?.Count > 0)
        {
            throw new ConflictException([$"不允许删除 '{role.Name}' 角色，当前已有用户在使用该角色。"]);
        }

        var result = await roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            throw new IdentityException(result.GetIdentityResultErrorDescrptions());
        }

        return role.Name;
    }

    public async Task<bool> IsExistsAsync(string name)
    {
        return await roleManager.RoleExistsAsync(name);
    }

    public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest updateRolePermissionsRequest)
    {
        var role = await roleManager.FindByIdAsync(updateRolePermissionsRequest.RoleId)
            ?? throw new NotFoundException(["角色不存在。"]);

        if (role.Name == RoleConstants.Admin)
        {
            throw new ConflictException([$"不允许修改 '{role.Name}' 角色的权限。"]);
        }

        if (multiTenantContextAccessor.MultiTenantContext.TenantInfo.Id != TenancyConstants.Root.Id)
        {
            updateRolePermissionsRequest.Permissions.RemoveAll(x => x.StartsWith("Permission.Tenants."));
        }

        var claims = await roleManager.GetClaimsAsync(role);

        foreach (var claim in claims.Where(x => !updateRolePermissionsRequest.Permissions.Any(y => y == x.Value)))
        {
            var result = await roleManager.RemoveClaimAsync(role, claim);
            if (!result.Succeeded)
            {
                throw new IdentityException(result.GetIdentityResultErrorDescrptions());
            }
        }

        foreach (var permission in updateRolePermissionsRequest.Permissions.Where(x => !claims.Any(y => y.Value == x)))
        {
            await applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
            {
                RoleId = role.Id,
                ClaimType = ClaimConstants.Permission,
                ClaimValue = permission,
            });

            await applicationDbContext.SaveChangesAsync();
        }

        return role.Id;
    }

    public async Task<List<RoleResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        return roles.Adapt<List<RoleResponse>>();
    }

    public async Task<RoleResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var role = await applicationDbContext.Roles
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException(["角色不存在。"]);
        return role.Adapt<RoleResponse>();
    }

    public async Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        var role = await GetByIdAsync(id, cancellationToken);

        role.Permissions = await applicationDbContext.RoleClaims
            .Where(x => x.RoleId == id && x.ClaimType == ClaimConstants.Permission)
            .Select(x => x.ClaimValue)
            .ToListAsync(cancellationToken);

        return role;
    }
}
