using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MBA.Conteudo.Application.Services;
using MBA.Conteudo.Domain.Interfaces;
using MBA.Core.DomainHadlers;
using MBA.Core.Messages;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MBA.Conteudo.Data.Repository;
using MBA.Core.Mediator;
using MBA.Conteudo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace MBA.Conteudo.Application.Configurations;

[ExcludeFromCodeCoverage]
public static class ConteudoConfiguration
{
	public static IServiceCollection ConfigurarConteudoApplication(this IServiceCollection services,
		string stringConexao, bool ehProducao)
	{
		return services
			.ConfigurarInjecoesDependenciasRepository()
			.ConfigurarInjecoesDependenciasApplication()
			.ConfigurarRepositorios(stringConexao, ehProducao);
	}

	private static IServiceCollection ConfigurarInjecoesDependenciasRepository(this IServiceCollection services)
	{
		services.AddScoped<IConteudoRepository, ConteudoRepository>();
		return services;
	}

	private static IServiceCollection ConfigurarInjecoesDependenciasApplication(this IServiceCollection services)
	{
		services.AddScoped<IMediatorHandler, MediatorHandler>();

		services.AddScoped<INotificationHandler<DomainNotificacaoRaiz>, DomainNotificacaoHandler>();

		services.AddScoped<ICursoAppService, CursoAppService>();
		services.AddScoped<IAulaAppService, AulaAppService>();

		return services;
	}

	private static IServiceCollection ConfigurarRepositorios(this IServiceCollection services, string stringConexao,
		bool ehProducao)
	{
		services.AddDbContext<ConteudoContext>(o =>
		{
			if (ehProducao)
			{
				o.UseSqlServer(stringConexao);
			}
			else
			{
				var connection = new SqliteConnection(stringConexao);
				connection.CreateCollation("LATIN1_GENERAL_CI_AI", (x, y) =>
					string.Compare(x, y, CultureInfo.CurrentCulture,
						CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace));

				o.UseSqlite(connection);
			}
		});

		return services;
	}
}