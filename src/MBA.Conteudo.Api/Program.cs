using MBA.Conteudo.Api.Configuration;
using MBA.Conteudo.Api.Settings;
using MBA.Conteudo.Application.Configurations;
using MBA.Core.Mediator;
using MBA.Core.Messages;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>()
	?? throw new InvalidOperationException("AppSettings section is missing from configuration.");


builder.Services.AddHttpContextAccessor()
	.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
		Assembly.GetExecutingAssembly(),
		typeof(DomainNotificacaoRaiz).Assembly
	))
	.AddScoped<IMediatorHandler, MediatorHandler>()
	.ConfigurarJwt(appSettings.JwtSettings)
	.ConfigurarConteudoApplication(appSettings.DatabaseSettings.ConnectionStringConteudo,
		builder.Environment.IsProduction())
	.ConfigurarApi()
	.ConfigurarCors()
	.AddSwaggerConfig();

var app = builder.Build();
app.ExecutarConfiguracaoAmbiente();
app.Run();