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

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
	Assembly.GetExecutingAssembly(),
	typeof(MBA.Aluno.Application.Services.AlunoAppService).Assembly
));

var app = builder.Build();

app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.Run();