using MBA.Bff.Api.Configuration;
using MBA.WebApi.Core.Extensions;
using MBA.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddMessageBusConfiguration(builder.Configuration);

// BFF é orquestrador (sem DbContext) — só liveness; readiness sem dependência própria.
builder.Services.AddDefaultHealthChecks();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration();
}

app.UseApiConfiguration(app.Environment);

app.MapDefaultHealthChecks();

app.UseDefaultMetrics();

app.Run();
