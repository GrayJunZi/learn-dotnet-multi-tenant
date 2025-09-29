using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMS.Domain.Entities;
using PMS.Infrastructure.Identity.Models;

namespace PMS.Infrastructure.Contexts;

internal class DbConfigurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users", "Identity")
                .IsMultiTenant();
        }
    }

    internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.ToTable("Roles", "Identity")
                .IsMultiTenant();
        }
    }

    internal class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
        {
            builder.ToTable("RoleClaims", "Identity")
                .IsMultiTenant();
        }
    }

    internal class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder.ToTable("UserRoles", "Identity")
                .IsMultiTenant();
        }
    }

    internal class ApplicationUserClaimConfiguration : IEntityTypeConfiguration<ApplicationUserClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
        {
            builder.ToTable("UserClaims", "Identity")
                .IsMultiTenant();
        }
    }

    internal class ApplicationUserLoginConfiguration : IEntityTypeConfiguration<ApplicationUserLogin>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
        {
            builder.ToTable("UserLogins", "Identity")
                .IsMultiTenant();
        }
    }

    internal class ApplicationUserTokenConfiguration : IEntityTypeConfiguration<ApplicationUserToken>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
        {
            builder.ToTable("UserTokens", "Identity")
                .IsMultiTenant();
        }
    }

    internal class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies", "Enterprise")
                .IsMultiTenant();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(60);


        }
    }
}
