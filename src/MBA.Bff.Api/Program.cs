using MBA.Bff.Api.Configuration;
using MBA.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddSwaggerConfiguration();

builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.RegisterServices();

builder.Services.AddMessageBusConfiguration(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiConfiguration(app.Environment);
app.UseSwaggerConfiguration();

app.Run();
