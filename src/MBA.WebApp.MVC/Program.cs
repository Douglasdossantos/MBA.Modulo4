using MBA.WebApp.MVC.Configuration;
using MBA.WebApp.MVC.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityConfiguration();
builder.Services.AddControllersWithViews();
builder.Services.RegisterServices();

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

app.Run();
