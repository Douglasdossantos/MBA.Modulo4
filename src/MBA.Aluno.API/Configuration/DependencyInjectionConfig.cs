using MBA.Aluno.API.Services;
using MBA.Aluno.Application.Commands.RegistrarAulaAssistida;
using MBA.Aluno.Application.Interfaces;
using MBA.Aluno.Application.Queries;
using MBA.Aluno.Application.Services;
using MBA.Aluno.Data.Repositories;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Authentications;
using MBA.Core.DomainHadlers;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.AlunoCommands;
using MBA.WebApi.Core.Extensions;

using MediatR;

namespace MBA.Aluno.API.Configuration;

public static class DependencyInjectionConfig
{
	public static void ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
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

		// Aula assistida
		services.AddScoped<IAulaAssistidaRepository, AulaAssistidaRepository>();
		services.AddScoped<IRequestHandler<RegistrarAulaAssistidaCommand, bool>, RegistrarAulaAssistidaCommandHandler>();

		// Integração com Conteúdo API
		services.AddTransient<AuthorizationForwardingHandler>();

		var conteudoUrl = configuration["AppSettings:ServicosExternos:ConteudoUrl"];
		if (string.IsNullOrWhiteSpace(conteudoUrl))
			throw new InvalidOperationException(
				"Configuração 'AppSettings:ServicosExternos:ConteudoUrl' é obrigatória para consultar a Conteúdo API.");

		services.AddHttpClient<IConteudoService, ConteudoService>(client =>
		{
			client.BaseAddress = new Uri(conteudoUrl);
			client.Timeout = TimeSpan.FromSeconds(10);
		})
		.AddHttpMessageHandler<AuthorizationForwardingHandler>()
		.AddPolicyHandler(PollyExtensions.EsperarTentar());
	}
}