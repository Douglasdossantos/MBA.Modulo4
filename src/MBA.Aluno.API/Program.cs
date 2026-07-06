using MBA.Aluno.API.Configuration;
using MBA.Aluno.Data.Context;
using MBA.WebApi.Core.Extensions;
using MBA.WebApi.Core.Identidade;

using SQLitePCL;

using System.Reflection;

// Alias para desambiguar da classe homônima em MBA.WebApi.Core.Identidade.
using AppSettings = MBA.Aluno.API.Configuration.AppSettings;

var builder = WebApplication.CreateBuilder(args);

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

app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.MapDefaultHealthChecks();

app.Run();