using MBA.Core.Autentications;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Financeiro.Application.Configurations;
using MBA.Pagamentos.Data.Contexts;
using MBA.Settings;
using Microsoft.EntityFrameworkCore;
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





builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
});

builder.Services.AddScoped<IAppIdentityUser, AppIdentityUser>();

builder.Services.AddHttpContextAccessor()
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    Assembly.GetExecutingAssembly(),
    typeof(DomainNotificacaoRaiz).Assembly
))
    .AddScoped<IMediatorHandler, MediatorHandler>()
    .ConfigurarFaturamentoApplication(appSettings?.DatabaseSettings?.ConnectionStringFaturamento ?? "", builder.Environment.IsProduction());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
