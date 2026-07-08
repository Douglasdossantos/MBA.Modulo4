using MBA.Bff.Api.Configuration;
using MBA.WebApi.Core.Extensions;
using MBA.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);

// Fail-fast: sem estes segredos (que vêm do Infisical) a aplicação não sobe e explica como corrigir.
builder.Configuration.ValidarSegredosObrigatorios(
	"AppSettings:Secret",
	"MessageQueueConnection:MessageBus");

builder.Services.AddApiConfiguration(builder.Configuration, builder.Environment);

builder.Services.AddSwaggerConfiguration();
builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddMessageBusConfiguration(builder.Configuration);

// BFF é orquestrador (sem DbContext) — só liveness; readiness sem dependência própria.
builder.Services.AddDefaultHealthChecks();


var app = builder.Build();

// Configure the HTTP request pipeline.
// Swagger LIGADO em todos os ambientes; para ocultar, definir SWAGGER_ENABLED=false e reiniciar o serviço.
if (app.Configuration["SWAGGER_ENABLED"] != "false")
{
    app.UseSwaggerConfiguration();
}

app.UseApiConfiguration(app.Environment);

app.MapDefaultHealthChecks();

app.UseDefaultMetrics();

app.Run();
