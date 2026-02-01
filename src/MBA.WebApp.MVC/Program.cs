using MBA.WebApp.MVC.Configuration;
using MBA.WebApp.MVC.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityConfiguration();
builder.Services.AddControllersWithViews();
builder.Services.RegisterServices();

var app = builder.Build();

app.UseMvcConfiguration(app.Environment);

app.Run();
