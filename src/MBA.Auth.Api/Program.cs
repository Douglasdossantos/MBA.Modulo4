using MBA.Auth.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabaseSelector();

builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

builder.Services.AddApiConfiguration(builder.Configuration);

var app = builder.Build();
app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.Run();
