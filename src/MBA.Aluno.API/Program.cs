using EasyNetQ;
using MBA.Aluno.Api.MigrationHelp;
using MBA.Aluno.API.Configuration;
using MBA.Aluno.API.Data;
using MBA.Aluno.API.Data.Repository;
using MBA.Aluno.API.Models;
using MBA.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddApiConfiguration(builder.Configuration);


builder.AddDatabaseSelector();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration); 
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddMessageBusConfiguration(builder.Configuration);

//builder.Services.AddEasyNetQ("host=localhost:5672");





var app = builder.Build();

app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.Run();
