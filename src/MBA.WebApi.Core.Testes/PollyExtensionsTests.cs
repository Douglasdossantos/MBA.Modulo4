using System.Net;

using FluentAssertions;

using MBA.WebApi.Core.Extensions;

using Polly.CircuitBreaker;

namespace MBA.WebApi.Core.Testes;

public class PollyExtensionsTests
{
	private static HttpResponseMessage Resposta(HttpStatusCode status) => new(status);

	[Fact]
	public void EsperarTentar_deve_criar_politica()
	{
		PollyExtensions.EsperarTentar().Should().NotBeNull();
	}

	[Fact]
	public async Task EsperarTentar_deve_retentar_apos_erro_transitorio()
	{
		var policy = PollyExtensions.EsperarTentar();
		var chamadas = 0;

		var resposta = await policy.ExecuteAsync(() =>
		{
			chamadas++;
			var status = chamadas == 1 ? HttpStatusCode.InternalServerError : HttpStatusCode.OK;
			return Task.FromResult(Resposta(status));
		});

		resposta.StatusCode.Should().Be(HttpStatusCode.OK);
		chamadas.Should().Be(2); // 1 falha + 1 retry
	}

	[Fact]
	public async Task EsperarTentar_nao_deve_retentar_quando_sucesso()
	{
		var policy = PollyExtensions.EsperarTentar();
		var chamadas = 0;

		await policy.ExecuteAsync(() =>
		{
			chamadas++;
			return Task.FromResult(Resposta(HttpStatusCode.OK));
		});

		chamadas.Should().Be(1);
	}

	[Fact]
	public void CircuitBreaker_deve_criar_politica()
	{
		PollyExtensions.CircuitBreaker().Should().NotBeNull();
	}

	[Fact]
	public async Task CircuitBreaker_deve_abrir_apos_falhas_consecutivas()
	{
		var policy = PollyExtensions.CircuitBreaker();
		Func<Task<HttpResponseMessage>> falha = () => Task.FromResult(Resposta(HttpStatusCode.InternalServerError));

		// 5 falhas tratadas consecutivas -> o circuito abre
		for (var i = 0; i < 5; i++)
			await policy.ExecuteAsync(falha);

		// a proxima chamada e curto-circuitada (nao chega a executar o delegate)
		Func<Task> proxima = () => policy.ExecuteAsync(falha);

		await proxima.Should().ThrowAsync<BrokenCircuitException>();
	}
}
