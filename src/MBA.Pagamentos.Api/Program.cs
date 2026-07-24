using MBA.Core.Authentications;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Pagamentos.Api.Configurations;
using MBA.Pagamentos.Api.MigrationHelper;
using MBA.Pagamentos.Api.Services;
using MBA.Pagamentos.Application.Configurations;
using MBA.Pagamentos.Application.Services;
using MBA.Pagamentos.Data.Contexts;
using MBA.WebApi.Core.Extensions;

using Microsoft.OpenApi.Models;

using SQLitePCL;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Fail-fast: sem estes segredos (que vêm do Infisical) a aplicação não sobe e explica como corrigir.
builder.Configuration.ValidarSegredosObrigatorios(
	"AppSettings:JwtSettings:Secret",
	"MessageQueueConnection:MessageBus");

Batteries.Init();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "MBA Pagamentos API",
		Description =
			"AVISO IMPORTANTE — SWAGGER EXPOSTO DE PROPÓSITO: Esta é uma aplicação acadêmica (MBA DevXpert, Módulo 5) avaliada por professores, e TODOS os ambientes (inclusive produção) expõem esta documentação para facilitar a consulta e a correção do trabalho. A equipe SABE que em uma aplicação real o Swagger NÃO deve ficar público em produção. Para ocultá-lo, por padrao o Swagger fica oculto em ambiente publicado; para exibi-lo defina SWAGGER_ENABLED=true."
	});
});


var configuration = builder.Configuration;
builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

builder.Services.AddMessageBusConfiguration(builder.Configuration);

builder.Services.AddScoped<IAppIdentityUser, AppIdentityUser>();
builder.Services.AddTransient<AuthorizationForwardingHandler>();

builder.Services.AddHttpClient<IAlunoService, AlunoService>(client =>
{
	client.BaseAddress = new Uri(appSettings!.ServicosExternos.AlunoUrl);
	client.Timeout = TimeSpan.FromSeconds(10);
})
.AddHttpMessageHandler<AuthorizationForwardingHandler>()
.AddPolicyHandler(PollyExtensions.EsperarTentar())
.AddPolicyHandler(PollyExtensions.CircuitBreaker());

builder.Services.AddHttpContextAccessor()
	.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
		Assembly.GetExecutingAssembly(),
		typeof(DomainNotificacaoRaiz).Assembly
	))
	.AddScoped<IMediatorHandler, MediatorHandler>()
	.ConfigurarJwt(appSettings!.JwtSettings)
	// Development => SQLite; qualquer outro ambiente (Staging/Production) => SQL Server,
	// alinhado ao DataBaseSelector do Auth/Aluno.
	.ConfigurarFaturamentoApplication(appSettings.DatabaseSettings.ConnectionStringFaturamento,
		!builder.Environment.IsDevelopment());

builder.Services.AddDefaultHealthChecks()
	.AddDbContextCheck<FaturamentoDbContext>("pagamentos-db", tags: [HealthCheckExtensions.ReadyTag]);

var app = builder.Build();

// Swagger seguro por padrao: ligado em Development ou com SWAGGER_ENABLED=true; desligado em ambiente publicado sem o flag.
// Para expor em ambiente publicado, defina SWAGGER_ENABLED=true no ConfigMap do ambiente.
if (app.Environment.IsDevelopment() || app.Configuration["SWAGGER_ENABLED"] == "true")
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
await scope.ServiceProvider.CarregamentoDadosAsync();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapDefaultHealthChecks();
app.UseDefaultMetrics();
app.Run();