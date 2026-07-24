using FluentAssertions;

using MBA.WebApi.Core.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MBA.WebApi.Core.Testes;

public class HealthCheckExtensionsTests
{
	[Fact]
	public void AddDefaultHealthChecks_deve_retornar_builder()
	{
		new ServiceCollection().AddDefaultHealthChecks().Should().NotBeNull();
	}

	[Fact]
	public async Task AddDefaultHealthChecks_deve_registrar_o_check_self_como_live_saudavel()
	{
		var services = new ServiceCollection();
		services.AddLogging();
		services.AddDefaultHealthChecks();

		using var provider = services.BuildServiceProvider();
		var servico = provider.GetRequiredService<HealthCheckService>();

		var relatorio = await servico.CheckHealthAsync(
			registro => registro.Tags.Contains(HealthCheckExtensions.LiveTag));

		relatorio.Status.Should().Be(HealthStatus.Healthy);
		relatorio.Entries.Should().ContainKey("self");
	}

	[Fact]
	public async Task Predicado_de_ready_nao_deve_incluir_o_check_live()
	{
		var services = new ServiceCollection();
		services.AddLogging();
		services.AddDefaultHealthChecks();

		using var provider = services.BuildServiceProvider();
		var servico = provider.GetRequiredService<HealthCheckService>();

		var relatorio = await servico.CheckHealthAsync(
			registro => registro.Tags.Contains(HealthCheckExtensions.ReadyTag));

		relatorio.Entries.Should().NotContainKey("self");
	}
}
