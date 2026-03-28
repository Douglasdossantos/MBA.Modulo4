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

builder.AddDatabaseSelector();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();
builder.Services.ResolveDependencies();

builder.Services.AddMessageBusConfiguration(builder.Configuration);

var configuration = builder.Configuration;
builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

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
