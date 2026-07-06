using MBA.WebApp.MVC.Configuration;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityConfiguration();
builder.Services.AddControllersWithViews();
builder.Services.RegisterServices(builder.Configuration);

// Front-end sem dependência própria: liveness/readiness simples (200) p/ as probes do K8s.
builder.Services.AddHealthChecks();

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}


var app = builder.Build();

app.UseMvcConfiguration(app.Environment);

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.UseHttpMetrics();
app.MapMetrics();

app.Run();
