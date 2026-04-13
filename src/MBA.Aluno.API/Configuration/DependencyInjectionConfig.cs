using MBA.Aluno.Application.Interfaces;
using MBA.Aluno.Application.Queries;
using MBA.Aluno.Application.Services;
using MBA.Aluno.Data.Repositories;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Authentications;
using MBA.Core.DomainHadlers;
using MBA.Core.Mediator;
using MBA.Core.Messages;

using MediatR;

namespace MBA.Aluno.API.Configuration;

public static class DependencyInjectionConfig
{
	public static void ResolveDependencies(this IServiceCollection services)
	{
		services.AddScoped<IAppIdentityUser, AppIdentityUser>();

		services.AddScoped<IAlunoAppService, AlunoAppService>();
		services.AddScoped<IAlunoRepository, AlunoRepository>();

		services.AddScoped<IMediatorHandler, MediatorHandler>();
		services.AddScoped<INotificationHandler<DomainNotificacaoRaiz>, DomainNotificacaoHandler>();

		// Aluno
		services.AddScoped<IAlunoRepository, AlunoRepository>();
		services.AddScoped<IAlunoAppService, AlunoAppService>();

		// Matricula
		services.AddScoped<IMatriculaRepository, MatriculaRepository>();
		services.AddScoped<IAlunoQuery, AlunoQueryService>();
	}
}