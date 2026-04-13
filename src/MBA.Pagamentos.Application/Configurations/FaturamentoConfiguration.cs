using MBA.Core.DomainHadlers;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.FaturamentoCommands;
using MBA.Core.Messages.FaturamentoEvents;
using MBA.Pagamentos.Application.Commands.RealizarPagamento;
using MBA.Pagamentos.Application.Events.GerarLinkPagamento;
using MBA.Pagamentos.Data.Contexts;
using MBA.Pagamentos.Data.Repositories;
using MBA.Pagamentos.Domain.Interfaces;

using MediatR;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MBA.Pagamentos.Application.Configurations;

[ExcludeFromCodeCoverage]
public static class FaturamentoConfiguration
{
	public static IServiceCollection ConfigurarFaturamentoApplication(this IServiceCollection services,
		string stringConexao, bool ehProducao)
	{
		return services
			.ConfigurarInjecoesDependenciasRepository()
			.ConfigurarInjecoesDependenciasApplication()
			.ConfigurarRepositorios(stringConexao, ehProducao);
	}

	private static IServiceCollection ConfigurarInjecoesDependenciasRepository(this IServiceCollection services)
	{
		services.AddScoped<IFaturamentoRepository, FaturamentoRepository>();
		return services;
	}

	private static IServiceCollection ConfigurarInjecoesDependenciasApplication(this IServiceCollection services)
	{
		services.AddScoped<IMediatorHandler, MediatorHandler>();

		services.AddScoped<INotificationHandler<DomainNotificacaoRaiz>, DomainNotificacaoHandler>();
		services.AddScoped<INotificationHandler<GerarLinkPagamentoEvent>, GerarLinkPagamentoEventHandler>();

		services.AddScoped<IRequestHandler<RealizarPagamentoCommand, bool>, RealizarPagamentoCommandHandler>();

		return services;
	}

	private static IServiceCollection ConfigurarRepositorios(this IServiceCollection services, string stringConexao,
		bool ehProducao)
	{
		services.AddDbContext<FaturamentoDbContext>(o =>
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