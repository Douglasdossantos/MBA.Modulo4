using MBA.Core.Authentications;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Pagamentos.Api.Configurations;
using MBA.Pagamentos.Api.MigrationHelper;
using MBA.Pagamentos.Application.Configurations;

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

builder.Services.AddMessageBusConfiguration(builder.Configuration);

builder.Services.AddScoped<IAppIdentityUser, AppIdentityUser>();

builder.Services.AddHttpContextAccessor()
	.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
		Assembly.GetExecutingAssembly(),
		typeof(DomainNotificacaoRaiz).Assembly
	))
	.AddScoped<IMediatorHandler, MediatorHandler>()
	.ConfigurarJwt(appSettings!.JwtSettings)
	.ConfigurarFaturamentoApplication(appSettings.DatabaseSettings.ConnectionStringFaturamento,
		builder.Environment.IsProduction());

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