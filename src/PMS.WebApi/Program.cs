using PMS.Application;
using PMS.Infrastructure;
using PMS.WebApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Services.GetJwtSettings(builder.Configuration));

builder.Services.AddApplication();

var app = builder.Build();

await app.Services.AddDatabaseInitializerAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseInfrastructure();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();