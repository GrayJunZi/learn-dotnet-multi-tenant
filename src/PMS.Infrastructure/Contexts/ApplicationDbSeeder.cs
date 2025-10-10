using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PMS.Core.Constants;
using PMS.Infrastructure.Identity.Models;
using PMS.Infrastructure.Tenancy;

namespace PMS.Infrastructure.Contexts;

public class ApplicationDbSeeder(
    IMultiTenantContextAccessor<CompanyTenantInfo> multiTenantContextAccessor,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ApplicationDbContext applicationDbContext)
{
    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        if (!applicationDbContext.Database.GetMigrations().Any())
            return;

        if ((await applicationDbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await applicationDbContext.Database.MigrateAsync(cancellationToken);
        }

        if (await applicationDbContext.Database.CanConnectAsync(cancellationToken))
        {
            await initializeDefaultRolesAsync(cancellationToken);
            await initializeAdminUserAsync(cancellationToken);
        }
    }

    private async Task initializeDefaultRolesAsync(CancellationToken cancellationToken)
    {
        foreach (var roleName in RoleConstants.DefaultRoles)
        {
            if (await roleManager.Roles.SingleOrDefaultAsync(role => role.Name == roleName, cancellationToken) is not ApplicationRole incomingRole)
            {
                incomingRole = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} Role",
                };

                await roleManager.CreateAsync(incomingRole);
            }

            switch (roleName)
            {
                case RoleConstants.Basic:
                    await assignPermissionsToRole(CompanyPermissions.Basic, incomingRole, cancellationToken);
                    break;
                case RoleConstants.Admin:
                    await assignPermissionsToRole(CompanyPermissions.Admin, incomingRole, cancellationToken);

                    if (multiTenantContextAccessor.MultiTenantContext.TenantInfo.Id == TenancyConstants.Root.Id)
                    {
                        await assignPermissionsToRole(CompanyPermissions.Root, incomingRole, cancellationToken);
                    }
                    break;
            }
        }
    }

    private async Task assignPermissionsToRole(
        IReadOnlyList<CompanyPermission> rolePermissions,
        ApplicationRole applicationRole,
        CancellationToken cancellationToken)
    {
        var claims = await roleManager.GetClaimsAsync(applicationRole);

        foreach (var rolePermission in rolePermissions)
        {
            if (!claims.Any(x => x.Type == ClaimConstants.Permission && x.Value == rolePermission.Name))
            {
                await applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = applicationRole.Id,
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = rolePermission.Name,
                    Description = rolePermission.Description,
                    Group = rolePermission.Group,
                }, cancellationToken);

                await applicationDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task initializeAdminUserAsync(CancellationToken cancellationToken)
    {
        var email = multiTenantContextAccessor.MultiTenantContext.TenantInfo.Email;
        if (string.IsNullOrEmpty(email))
            return;

        if (await userManager.Users.SingleOrDefaultAsync(user => user.Email == email, cancellationToken) is not ApplicationUser incomingUser)
        {
            incomingUser = new ApplicationUser
            {
                Name = multiTenantContextAccessor.MultiTenantContext.TenantInfo.Name,
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = email.ToUpperInvariant(),
                NormalizedUserName = email.ToUpperInvariant(),
                IsActive = true,
            };

            var passwordHash = new PasswordHasher<ApplicationUser>();
            incomingUser.PasswordHash = passwordHash.HashPassword(incomingUser, TenancyConstants.DefaultPassword);
            await userManager.CreateAsync(incomingUser);
        }

        if (!await userManager.IsInRoleAsync(incomingUser, RoleConstants.Admin))
        {
            await userManager.AddToRoleAsync(incomingUser, RoleConstants.Admin);
        }
    }
}
