using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PMS.Infrastructure.Contexts;
using PMS.Infrastructure.Tenancy;

namespace PMS.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<TenantDbContext>(options => options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection")))
            .AddMultiTenant<CompanyTenantInfo>()
                .WithHeaderStrategy(TenancyConstants.TenantIdName)
                .WithClaimStrategy(TenancyConstants.TenantIdName)
                .WithEFCoreStore<TenantDbContext, CompanyTenantInfo>()
                .Services
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app
            .UseMultiTenant();
    }
}
