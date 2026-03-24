
using MBA.API.Configurations;
using MBA.Conteudo.Api.Configuration;
using MBA.Conteudo.API.Configurations;
using MBA.Conteudo.API.Settings;
using MBA.Conteudo.Application.Configurations;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();


builder.Services.AddHttpContextAccessor()
    .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())

    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    Assembly.GetExecutingAssembly(),
    typeof(DomainNotificacaoRaiz).Assembly
))
    .AddScoped<IMediatorHandler, MediatorHandler>()

    .ConfigurarJwt(appSettings.JwtSettings)
    //.ConfigurarAlunoApplication(appSettings.DatabaseSettings.ConnectionStringAluno, builder.Environment.IsProduction())
    .ConfigurarConteudoApplication(appSettings.DatabaseSettings.ConnectionStringConteudo, builder.Environment.IsProduction())
   // .ConfigurarFaturamentoApplication(appSettings.DatabaseSettings.ConnectionStringFaturamento, builder.Environment.IsProduction())
    .ConfigurarApi()
    .ConfigurarCors()
    .AddSwaggerConfig();

var app = builder.Build();
app.ExecutarConfiguracaoAmbiente();
app.Run();