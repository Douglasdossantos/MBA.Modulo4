using MBA.Aluno.API.Data.Repository;
using MBA.Aluno.API.Data;
using MBA.Aluno.API.Models;
using MBA.Core.Mediator;
using MBA.Aluno.API.Application.Commands;
using FluentValidation.Results;
using MediatR;
using MBA.Aluno.API.Application.Events;
using MBA.Aluno.API.Services;

namespace MBA.Aluno.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegistarServices(this IServiceCollection services)
        {
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            services.AddScoped<AlunoContext>();

            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IRequestHandler<RegistarAlunoCommand, ValidationResult>, AlunoCommandHandler>();

            services.AddScoped<INotificationHandler<AlunoRegistradoEvent>, AlunoEventHandler>();

        }
    }
}
