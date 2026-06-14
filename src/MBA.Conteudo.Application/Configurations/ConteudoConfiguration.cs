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
		// T-14: provider explícito via env var (DATABASE_PROVIDER=SqlServer|Sqlite) vence; sem ela,
		// mantém o fallback atual (Production => SQL Server).
		var useSqlServer = System.Environment.GetEnvironmentVariable("DATABASE_PROVIDER") switch
		{
			"SqlServer" => true,
			"Sqlite" => false,
			_ => ehProducao
		};

		services.AddDbContext<ConteudoContext>(o =>
		{
			if (useSqlServer)
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