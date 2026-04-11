using MBA.Configurations;
using MBA.Core.Autentications;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Financeiro.Application.Configurations;
using MBA.Pagamentos.Api.Configurations;
using MBA.Pagamentos.Api.MigrationHelper;
using MBA.Settings;

using SQLitePCL;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Batteries.Init();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var configuration = builder.Configuration;
builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies()); });
builder.Services.AddMessageBusConfiguration(builder.Configuration);

builder.Services.AddScoped<IAppIdentityUser, AppIdentityUser>();

var isProduction = builder.Environment.IsProduction();
var connectionString = appSettings?.DatabaseSettings?.ConnectionStringFaturamento ?? "";

if (!isProduction)
{
	var loggerFactory = LoggerFactory.Create(lb => lb.AddConsole());
	var startupLogger = loggerFactory.CreateLogger("Startup");

	var resolvedFolder = SqlitePathResolver.ResolveDatabaseFolder(
		appSettings?.DatabaseSettings?.SqliteFolderPath,
		startupLogger);

	connectionString = SqlitePathResolver.BuildConnectionString(resolvedFolder, startupLogger);
}

builder.Services.AddHttpContextAccessor()
	.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
		Assembly.GetExecutingAssembly(),
		typeof(DomainNotificacaoRaiz).Assembly
	))
	.AddScoped<IMediatorHandler, MediatorHandler>()
	.ConfigurarJwt(appSettings.JwtSettings)
	.ConfigurarFaturamentoApplication(connectionString, isProduction);

var app = builder.Build();

if (app.Environment.IsDevelopment())
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
app.Run();