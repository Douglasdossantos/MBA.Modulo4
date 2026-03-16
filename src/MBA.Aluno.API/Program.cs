using FluentValidation.Results;
using MBA.Aluno.API.Configuration;
using MBA.Aluno.Appplication.Commands.CadastroAluno;
using MBA.Aluno.Appplication.Commands.ConcluirCurso;
using MBA.Aluno.Appplication.Commands.Matricular;
using MBA.Aluno.Appplication.Interfaces;
using MBA.Aluno.Appplication.Queries;
using MBA.Aluno.Appplication.Services;
using MBA.Aluno.Data.Context;
using MBA.Aluno.Data.Repositories;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Autentications;
using MBA.Core.DomainHadlers;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.AlunoCommands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Batteries.Init();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMessageBusConfiguration(builder.Configuration);

var configuration = builder.Configuration;
builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAppIdentityUser, AppIdentityUser>();

builder.Services.AddScoped<IAlunoAppService, AlunoAppService>();
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();

//builder.Services.AddScoped<IRequestHandler<RegistrarAlunoCommand, bool>, RegistrarAlunoCommandHandler>();




builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
});

builder.Services.AddDbContext<AlunoDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("ConnectionStringAluno")));

builder.Services.AddScoped<IMediatorHandler, MediatorHandler>();
builder.Services.AddScoped<INotificationHandler<DomainNotificacaoRaiz>, DomainNotificacaoHandler>();

// Aluno
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
builder.Services.AddScoped<IAlunoAppService, AlunoAppService>();

// Aluno - Commands Handlers
builder.Services.AddScoped<IRequestHandler<CadastroAlunoCommand, ValidationResult>, CadastroAlunoCommandHandler>();
//builder.Services.AddScoped<IRequestHandler<RegistrarAulaAssistidaCommand, bool>, RegistrarAulaAssistidaCommandHandler>();
builder.Services.AddScoped<IRequestHandler<ConcluirCursoCommand, bool>, ConcluirCursoCommandHandler>();

// Matricula
builder.Services.AddScoped<IMatriculaRepository, MatriculaRepository>();
builder.Services.AddScoped<IMatriculaAppService, MatriculaAppService>();
builder.Services.AddScoped<IAlunoQuery, AlunoQueryService>();
builder.Services.AddScoped<IRequestHandler<MatricularAlunoCommand, bool>, MatricularAlunoCommandHandler>();




builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
