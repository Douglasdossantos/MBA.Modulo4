using MBA.Aluno.API.Configuration;
using MBA.Aluno.Data.Context;
using MBA.WebApi.Core.Extensions;
using MBA.WebApi.Core.Identidade;

using SQLitePCL;

using System.Reflection;

// Alias para desambiguar da classe homônima em MBA.WebApi.Core.Identidade.
using AppSettings = MBA.Aluno.API.Configuration.AppSettings;

var builder = WebApplication.CreateBuilder(args);

// Fail-fast: sem estes segredos (que vêm do Infisical) a aplicação não sobe e explica como corrigir.
builder.Configuration.ValidarSegredosObrigatorios(
	"AppSettings:Secret",
	"MessageQueueConnection:MessageBus");

Batteries.Init();

builder.AddDatabaseSelector();

builder.Services.AddControllers();

builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();

builder.Services.ResolveDependencies(builder.Configuration);

builder.Services.AddMessageBusConfiguration(builder.Configuration);

var configuration = builder.Configuration;

builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

builder.Services.AddHttpContextAccessor();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
	Assembly.GetExecutingAssembly(),
	typeof(MBA.Aluno.Application.Services.AlunoAppService).Assembly
));

builder.Services.AddDefaultHealthChecks()
	.AddDbContextCheck<AlunoDbContext>("aluno-db", tags: [HealthCheckExtensions.ReadyTag]);

var app = builder.Build();

// Swagger LIGADO em todos os ambientes por decisão acadêmica (ver banner em SwaggerConfig).
// Único gate: ocultar apenas quando SWAGGER_ENABLED estiver definido exatamente como "false".
if (app.Configuration["SWAGGER_ENABLED"] != "false")
{
	app.UseSwaggerConfiguration();
}

app.UseApiConfiguration(app.Environment);

app.MapDefaultHealthChecks();

app.UseDefaultMetrics();

app.Run();