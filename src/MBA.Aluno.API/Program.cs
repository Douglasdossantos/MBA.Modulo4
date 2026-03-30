using MBA.Aluno.API.Configuration;
using SQLitePCL;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Batteries.Init();

builder.AddDatabaseSelector();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();

builder.Services.ResolveDependencies();

builder.Services.AddMessageBusConfiguration(builder.Configuration);

var configuration = builder.Configuration;

builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
});

builder.Services.ResolveDependencies();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

var app = builder.Build();

app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.Run();
