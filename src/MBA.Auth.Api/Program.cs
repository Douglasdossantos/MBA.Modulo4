using MBA.Auth.Api.Configuration;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabaseSelector();

builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddMessageBusConfiguration(builder.Configuration);

var app = builder.Build();
app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.Run();
