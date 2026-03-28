using MBA.Aluno.Appplication.Commands.CadastroAluno;
using MBA.Aluno.Appplication.Commands.ConcluirCurso;
using MBA.Aluno.Appplication.Commands.Matricular;
using MBA.Aluno.Appplication.Interfaces;
using MBA.Aluno.Appplication.Queries;
using MBA.Aluno.Appplication.Services;
using MBA.Aluno.Data.Repositories;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Autentications;
using MBA.Core.DomainHadlers;
using MBA.Core.Mediator;
using MBA.Core.Messages.AlunoCommands;
using MBA.Core.Messages;
using MediatR;
using FluentValidation.Results;

namespace MBA.Aluno.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services) 
        {
           services.AddScoped<IAppIdentityUser, AppIdentityUser>();

           services.AddScoped<IAlunoAppService, AlunoAppService>();
           services.AddScoped<IAlunoRepository, AlunoRepository>();

            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<INotificationHandler<DomainNotificacaoRaiz>, DomainNotificacaoHandler>();

            // Aluno
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            services.AddScoped<IAlunoAppService, AlunoAppService>();

            // Aluno - Commands Handlers
            services.AddScoped<IRequestHandler<CadastroAlunoCommand, ValidationResult>, CadastroAlunoCommandHandler>();
            //builder.Services.AddScoped<IRequestHandler<RegistrarAulaAssistidaCommand, bool>, RegistrarAulaAssistidaCommandHandler>();
            services.AddScoped<IRequestHandler<ConcluirCursoCommand, bool>, ConcluirCursoCommandHandler>();

            // Matricula
            services.AddScoped<IMatriculaRepository, MatriculaRepository>();
            services.AddScoped<IMatriculaAppService, MatriculaAppService>();
            services.AddScoped<IAlunoQuery, AlunoQueryService>();
            services.AddScoped<IRequestHandler<MatricularAlunoCommand, bool>, MatricularAlunoCommandHandler>();

            return services;
        }
    }
}
