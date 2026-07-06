using System.Net;
using FluentAssertions;
using MBA.SmokeTests.Infraestrutura;
using Xunit.Abstractions;

namespace MBA.SmokeTests;

/// <summary>
/// Verifica a saúde (liveness) dos serviços. Auth, Aluno, Pagamentos e BFF expõem /health/live;
/// a Conteúdo API não expõe (PEGADINHA 1) e por isso é validada por um teste funcional separado.
/// </summary>
public class HealthChecksTests : IClassFixture<SmokeTestFixture>
{
    private readonly SmokeTestFixture _fixture;
    private readonly ITestOutputHelper _saida;

    public HealthChecksTests(SmokeTestFixture fixture, ITestOutputHelper saida)
    {
        _fixture = fixture;
        _saida = saida;
    }

    // Serviços que registram health checks e devem responder 200 em /health/live.
    // A Conteúdo API NÃO entra aqui de propósito (ver teste funcional abaixo, PEGADINHA 1).
    [SmokeTheory]
    [InlineData("Auth")]
    [InlineData("Aluno")]
    [InlineData("Pagamentos")]
    [InlineData("Bff")]
    public async Task Deve_responder_health_live_Quando_servico_expoe_healthcheck(string servico)
    {
        var client = _fixture.PorNome(servico);
        using var cts = SmokeTestFixture.NovaCts();

        var resposta = await client.GetAsync("/health/live", cts.Token);
        _saida.WriteLine($"[{servico}] GET /health/live => {(int)resposta.StatusCode} {resposta.StatusCode}");

        resposta.StatusCode.Should().Be(
            HttpStatusCode.OK,
            because: $"o serviço {servico} deve expor /health/live saudável quando o ambiente está no ar");
    }

    // PEGADINHA 1: a Conteúdo API não registra health checks em Program.cs, então /health/live
    // retorna 404 para sempre e o container fica unhealthy no compose. Aqui validamos que o serviço
    // está VIVO por uma chamada funcional: GET /api/Curso sem token deve devolver ALGUMA resposta HTTP
    // (tipicamente 401/403 por falta de claim, ou 200). O simples fato de a chamada completar sem
    // exceção/timeout já comprova que o processo responde.
    [SmokeFact]
    public async Task Deve_responder_endpoint_funcional_Quando_conteudo_api_nao_tem_healthcheck()
    {
        using var cts = SmokeTestFixture.NovaCts();

        var resposta = await _fixture.Conteudo.GetAsync("/api/Curso", cts.Token);
        _saida.WriteLine($"[Conteudo] GET /api/Curso (sem token) => {(int)resposta.StatusCode} {resposta.StatusCode}");

        ((int)resposta.StatusCode).Should().BeInRange(
            200, 499,
            because: "qualquer resposta HTTP (ex.: 401/403/200) comprova que a Conteúdo API está viva, "
                   + "mesmo sem endpoint de health (PEGADINHA 1)");
    }
}
