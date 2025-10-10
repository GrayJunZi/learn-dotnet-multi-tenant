using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PMS.WebApp;
using PMS.WebApp.Infrastructure;
using PMS.WebApp.Infrastructure.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton(builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>());

builder.AddClient();

await builder.Build().RunAsync();
