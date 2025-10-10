using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using PMS.Core.Constants;
using PMS.WebApp.Infrastructure.Services.Auth;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace PMS.WebApp.Infrastructure.Extensions;

public static class WasmHostBuilderExtensions
{
    private const string _httpClientName = "PMS API";

    public static WebAssemblyHostBuilder AddClient(this WebAssemblyHostBuilder builder)
    {
        builder.Services
            .AddAuthorizationCore(RegisterPermissions)
            .AddBlazoredLocalStorage()
            .AddMudServices(configuration =>
            {
                configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                configuration.SnackbarConfiguration.VisibleStateDuration = 5000;
                configuration.SnackbarConfiguration.ShowCloseIcon = true;
            })
            .AddScoped<ApplicationStateProvider>()
            .AddScoped<AuthenticationStateProvider, ApplicationStateProvider>()
            .AddTransient<AuthenticationHeaderHandler>()
            .AddHttpClientInterceptor();

        builder.Services
            .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
            .CreateClient(_httpClientName).EnableIntercept(sp))
            .AddHttpClient(_httpClientName, client =>
            {
                client.BaseAddress = new Uri(builder.Configuration.GetSection("ApiSettings:BaseApiUrl").Get<string>());
            })
            .AddHttpMessageHandler<AuthenticationHeaderHandler>();

        return builder;
    }

    private static void RegisterPermissions(AuthorizationOptions options)
    {
        foreach (var permission in CompanyPermissions.All)
        {
            options.AddPolicy(permission.Name, policy => policy.RequireClaim(ClaimConstants.Permission, permission.Name));
        }
    }
}
