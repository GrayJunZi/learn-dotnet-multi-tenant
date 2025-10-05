using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PMS.Application;
using PMS.Application.Features.Companies;
using PMS.Application.Features.Identity.Tokens;
using PMS.Application.Features.Tenancy;
using PMS.Application.Wrappers;
using PMS.Infrastructure.Companies;
using PMS.Infrastructure.Constants;
using PMS.Infrastructure.Contexts;
using PMS.Infrastructure.Identity.Auth;
using PMS.Infrastructure.Identity.Models;
using PMS.Infrastructure.Identity.Tookens;
using PMS.Infrastructure.OpenApi;
using PMS.Infrastructure.Tenancy;
using System.Net;
using System.Security.Claims;
using System.Text;

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
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")))
            .AddTransient<ITenantDbSeeder, TenantDbSeeder>()
            .AddTransient<ApplicationDbSeeder>()
            .AddTransient<ITenantService, TenantService>()
            .AddTransient<ICompanyService, CompanyService>()
            .AddIdentityService()
            .AddPermissions()
            .AddOpenApiDocumentation(configuration);
    }

    public static async Task AddDatabaseInitializerAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        await scope.ServiceProvider.GetRequiredService<ITenantDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);
    }

    internal static IServiceCollection AddIdentityService(this IServiceCollection services)
    {
        return services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .Services
            .AddScoped<ITokenService, TokenService>();
    }

    internal static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
    }

    public static JwtSettings GetJwtSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettinsConfiguration = configuration.GetSection(nameof(JwtSettings));
        services.Configure<JwtSettings>(jwtSettinsConfiguration);

        return jwtSettinsConfiguration.Get<JwtSettings>();
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
    {
        var secret = Encoding.UTF8.GetBytes(jwtSettings.Secret);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                };

                bearer.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Response.HasStarted)
                        {
                            return Task.CompletedTask;
                        }

                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsJsonAsync(ResponseWrapper.Fail("Token 已过期。"));
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsJsonAsync(ResponseWrapper.Fail("发生内部错误。"));
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";

                            return context.Response.WriteAsJsonAsync(ResponseWrapper.Fail("无权限。"));
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsJsonAsync(ResponseWrapper.Fail("无权限访问该资源。"));
                    },
                };
            });

        services.AddAuthorization(options =>
        {
            foreach (var property in typeof(CompanyPermissions).GetNestedTypes()
                .SelectMany(x => x.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = property.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy
                        .RequireClaim(ClaimConstants.Permission, propertyValue.ToString()));
                }
            }
        });

        return services;
    }

    internal static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        var scalarSettings = configuration.GetSection(nameof(ScalarSettings))
            .Get<ScalarSettings>();

        services.AddEndpointsApiExplorer();
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = scalarSettings.Title,
                    Description = scalarSettings.Description,
                    Version = scalarSettings.Version,
                    Contact = new()
                    {
                        Name = scalarSettings.ContactName,
                        Email = scalarSettings.ContactEmail,
                    },
                    License = new()
                    {
                        Name = scalarSettings.LicenseName,
                        Url = scalarSettings.LicenseUrl?.Length > 0 ? new Uri(scalarSettings.LicenseUrl) : null,
                    },
                };

                document.Components ??= new();
                document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                {
                    [JwtBearerDefaults.AuthenticationScheme] = new()
                    {
                        Name = "Authorization",
                        Description = "Enter your Bearer token to attach it as a header on your requests.",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT",
                    }
                };

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app
            .UseAuthentication()
            .UseAuthorization()
            .UseMultiTenant();
    }
}
