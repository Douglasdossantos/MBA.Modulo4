using MBA.Auth.Api.Configuration;
using MBA.WebApi.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabaseSelector();

builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddMessageBusConfiguration(builder.Configuration);

var app = builder.Build();
app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.UseDefaultMetrics();

app.Run();