using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using MBA.SmokeTests.Infraestrutura;
using Xunit.Abstractions;

namespace MBA.SmokeTests;

/// <summary>
/// Teste E2E (caixa-preta) do fluxo principal: registro do aluno, login admin, listagem de cursos,
/// matrícula, descoberta do matriculaId, pagamento e confirmação assíncrona via polling do status.
/// Segue exatamente as rotas e payloads do dossiê técnico e documenta as pegadinhas conhecidas.
/// </summary>
public class FluxoMatriculaPagamentoTests : IClassFixture<SmokeTestFixture>
{
    private const int StatusPendentePagamento = 1;
    private const int StatusPagamentoRealizado = 2;
    private const int StatusPagamentoRecusado = 5;
    private const int TimeoutPollingSegundos = 60;
    private const int IntervaloPollingSegundos = 3;

    private readonly SmokeTestFixture _fixture;
    private readonly ITestOutputHelper _saida;

    public FluxoMatriculaPagamentoTests(SmokeTestFixture fixture, ITestOutputHelper saida)
    {
        _fixture = fixture;
        _saida = saida;
    }

    [SmokeFact]
    public async Task Deve_concluir_fluxo_matricula_e_pagamento_Quando_ambiente_completo_no_ar()
    {
        // ---------------------------------------------------------------------
        // Passo 1 - Registro do aluno (Auth API). E-mail único por execução via Guid.
        // O registro dispara RPC síncrono via RabbitMQ para a Aluno API (cria o aluno).
        // ---------------------------------------------------------------------
        var email = $"smoke+{Guid.NewGuid():N}@teste.com";
        const string senha = "Smoke@2026!";

        var corpoRegistro = new
        {
            nomeUsuario = "Smoke Teste Automatizado",
            email,
            senha,
            senhaConfirmacao = senha,
            administrador = false
        };

        var (respostaRegistro, corpoRegistroTexto) =
            await EnviarAsync(_fixture.Auth, HttpMethod.Post, "/api/identidade/nova-conta", token: null, corpoRegistro);

        _saida.WriteLine($"[1-Registro] POST /api/identidade/nova-conta ({email}) => {(int)respostaRegistro.StatusCode}");
        respostaRegistro.IsSuccessStatusCode.Should().BeTrue(
            because: $"o registro do aluno deve ser aceito. Corpo: {corpoRegistroTexto}");

        var (tokenAluno, alunoId) = ExtrairTokenEUsuario(corpoRegistroTexto);
        alunoId.Should().NotBeNullOrWhiteSpace(because: "o alunoId corresponde ao usuarioToken.id retornado no registro");
        _saida.WriteLine($"[1-Registro] alunoId = {alunoId}");

        // ---------------------------------------------------------------------
        // Passo 2 - Login do admin seed (adm@adm.com / Adm@2026!). O token admin é
        // necessário para listar cursos e serve de fallback no pagamento (PEGADINHA 3).
        // ---------------------------------------------------------------------
        var corpoLoginAdmin = new { email = "adm@adm.com", senha = "Adm@2026!" };
        var (respostaLogin, corpoLoginTexto) =
            await EnviarAsync(_fixture.Auth, HttpMethod.Post, "/api/identidade/autenticar", token: null, corpoLoginAdmin);

        _saida.WriteLine($"[2-LoginAdmin] POST /api/identidade/autenticar => {(int)respostaLogin.StatusCode}");
        respostaLogin.IsSuccessStatusCode.Should().BeTrue(
            because: $"o login do admin seed deve funcionar. Corpo: {corpoLoginTexto}");

        var (tokenAdmin, _) = ExtrairTokenEUsuario(corpoLoginTexto);
        tokenAdmin.Should().NotBeNullOrWhiteSpace(because: "o token admin é usado para listar cursos");

        // ---------------------------------------------------------------------
        // Passo 3 - Listar cursos (Conteúdo API) com token ADMIN.
        // [ClaimsAuthorize("Cursos","VI")]: aluno comum leva 403; admin passa por bypass de role.
        // Seed cria cursos com IDs aleatórios => obter cursoId/valor SEMPRE dinamicamente.
        // ---------------------------------------------------------------------
        var (respostaCursos, corpoCursosTexto) =
            await EnviarAsync(_fixture.Conteudo, HttpMethod.Get, "/api/Curso", tokenAdmin, corpo: null);

        _saida.WriteLine($"[3-Cursos] GET /api/Curso (token admin) => {(int)respostaCursos.StatusCode}");
        respostaCursos.IsSuccessStatusCode.Should().BeTrue(
            because: $"a listagem de cursos com token admin deve retornar 200. Corpo: {corpoCursosTexto}");

        var (cursoId, nomeCurso, valorCurso) = ExtrairPrimeiroCurso(corpoCursosTexto);
        cursoId.Should().NotBeNullOrWhiteSpace(because: "é preciso um cursoId válido para matricular");
        _saida.WriteLine($"[3-Cursos] cursoId = {cursoId}; nome = {nomeCurso}; valor = {valorCurso}");

        // ---------------------------------------------------------------------
        // Passo 4 - Matricular (Aluno API). Endpoint sem auth efetiva. Status inicial: PendentePagamento.
        // ---------------------------------------------------------------------
        var corpoMatricula = new { cursoId, alunoId };
        var (respostaMatricula, corpoMatriculaTexto) =
            await EnviarAsync(_fixture.Aluno, HttpMethod.Post, "/api/Aluno/matricular-aluno", token: null, corpoMatricula);

        _saida.WriteLine($"[4-Matricula] POST /api/Aluno/matricular-aluno => {(int)respostaMatricula.StatusCode}");
        respostaMatricula.IsSuccessStatusCode.Should().BeTrue(
            because: $"a matrícula deve ser criada (201). Corpo: {corpoMatriculaTexto}");

        // ---------------------------------------------------------------------
        // Passo 5 - Descobrir matriculaId (Aluno API): GET {alunoId}/PorId, filtrar por cursoId.
        // Endpoint SEM [ClaimsAuthorize], portanto funciona apesar da PEGADINHA 2.
        // ---------------------------------------------------------------------
        var (matriculaId, statusInicial) = await ObterMatriculaAsync(alunoId!, cursoId!);
        matriculaId.Should().NotBeNullOrWhiteSpace(
            because: "a matrícula recém-criada deve aparecer em matriculas[] filtrada por cursoId");
        _saida.WriteLine($"[5-MatriculaId] matriculaId = {matriculaId}; status inicial = {statusInicial}");
        statusInicial.Should().Be(StatusPendentePagamento,
            because: "logo após matricular o status deve ser PendentePagamento (1)");

        // ---------------------------------------------------------------------
        // Passo 6 - Pagar (Pagamentos API). Estratégia da PEGADINHA 3:
        // tentar primeiro com o token do ALUNO (fluxo desenhado). O endpoint exige DUAS claims
        // (Administrador/PG E Alunos/PG); aluno comum não tem Administrador/PG => risco de 403.
        // Se vier 403, registramos o FINDING e repetimos com token ADMIN (bypass de role) para
        // validar o restante do pipeline (evento -> consumer -> status).
        // ---------------------------------------------------------------------
        var corpoPagamento = new
        {
            alunoId,
            cursoId,
            matriculaCursoId = matriculaId,
            pagamentoPodeSerRealizado = true,
            nomeCurso,
            dataMatricula = DateTime.UtcNow.ToString("o"),
            dataConclusao = (string?)null,
            estadoMatricula = "PendentePagamento",
            valor = valorCurso,
            numeroCartao = "5502093788528294",
            nomeTitularCartao = "Smoke Teste",
            validadeCartao = "12/29",
            cvvCartao = "123"
        };

        var rotaPagamento = $"/api/Faturamento/{alunoId}/registrar-pagamento";
        var (respostaPagamento, corpoPagamentoTexto) =
            await EnviarAsync(_fixture.Pagamentos, HttpMethod.Post, rotaPagamento, tokenAluno, corpoPagamento);

        _saida.WriteLine($"[6-Pagamento] POST {rotaPagamento} (token aluno) => {(int)respostaPagamento.StatusCode}");

        if (respostaPagamento.StatusCode == HttpStatusCode.Forbidden)
        {
            _saida.WriteLine("[6-Pagamento][FINDING] PEGADINHA 3 confirmada em runtime: token do aluno recebeu 403 "
                           + "(falta a claim Administrador/PG exigida em AND). Refazendo com token admin.");
            (respostaPagamento, corpoPagamentoTexto) =
                await EnviarAsync(_fixture.Pagamentos, HttpMethod.Post, rotaPagamento, tokenAdmin, corpoPagamento);
            _saida.WriteLine($"[6-Pagamento] POST {rotaPagamento} (token admin) => {(int)respostaPagamento.StatusCode}");
        }

        respostaPagamento.IsSuccessStatusCode.Should().BeTrue(
            because: "o registro do pagamento deve ser aceito (com token do aluno ou, em fallback, do admin). "
                   + $"Corpo: {corpoPagamentoTexto}");

        // ---------------------------------------------------------------------
        // Passo 7 - Confirmação assíncrona. O consumer da Aluno API muda o status para
        // PagamentoRealizado (2) após o PagamentoConfirmadoIntegrationEvent. Polling com timeout de 60s.
        // Obs.: se a PEGADINHA 2 quebrar a validação interna (Aluno API sem esquema JWT), o status
        // pode nunca chegar a 2 (ficando em 1) ou virar PagamentoRecusado (5); nesse caso o teste
        // FALHA de propósito, expondo onde o fluxo parou (não mascaramos o problema).
        // ---------------------------------------------------------------------
        var statusFinal = await AguardarStatusAsync(alunoId!, cursoId!, StatusPagamentoRealizado);
        _saida.WriteLine($"[7-Polling] status final após até {TimeoutPollingSegundos}s = {statusFinal} "
                       + $"({DescreverStatus(statusFinal)})");

        statusFinal.Should().Be(StatusPagamentoRealizado,
            because: $"após o pagamento confirmado o status da matrícula deve ser PagamentoRealizado (2); "
                   + $"obtido {statusFinal} ({DescreverStatus(statusFinal)})");
    }

    // ------------------------------------------------------------------------
    // Auxiliares
    // ------------------------------------------------------------------------

    /// <summary>
    /// Envia uma requisição HTTP com header Authorization opcional e corpo JSON opcional,
    /// sempre com CancellationToken por requisição (timeout de saída). Retorna a resposta e o corpo textual.
    /// </summary>
    private static async Task<(HttpResponseMessage resposta, string corpo)> EnviarAsync(
        HttpClient client, HttpMethod metodo, string rota, string? token, object? corpo)
    {
        using var cts = SmokeTestFixture.NovaCts();
        using var requisicao = new HttpRequestMessage(metodo, rota);

        if (!string.IsNullOrWhiteSpace(token))
        {
            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (corpo is not null)
        {
            var json = JsonSerializer.Serialize(corpo, SmokeTestFixture.JsonOpcoes);
            requisicao.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        var resposta = await client.SendAsync(requisicao, cts.Token);
        var texto = await resposta.Content.ReadAsStringAsync(cts.Token);
        return (resposta, texto);
    }

    /// <summary>Lê accessToken e usuarioToken.id de uma resposta da Auth API (sem envelope).</summary>
    private static (string? token, string? usuarioId) ExtrairTokenEUsuario(string corpo)
    {
        using var doc = JsonDocument.Parse(corpo);
        var raiz = doc.RootElement;

        var token = raiz.TryGetProperty("accessToken", out var t) ? t.GetString() : null;
        string? usuarioId = null;
        if (raiz.TryGetProperty("usuarioToken", out var ut) && ut.TryGetProperty("id", out var id))
        {
            usuarioId = id.GetString();
        }

        return (token, usuarioId);
    }

    /// <summary>Lê o primeiro curso do envelope { success, result: [...] } da Conteúdo API.</summary>
    private static (string? cursoId, string? nome, double valor) ExtrairPrimeiroCurso(string corpo)
    {
        using var doc = JsonDocument.Parse(corpo);
        if (!doc.RootElement.TryGetProperty("result", out var result)
            || result.ValueKind != JsonValueKind.Array
            || result.GetArrayLength() == 0)
        {
            return (null, null, 0d);
        }

        var curso = result[0];
        var cursoId = curso.TryGetProperty("id", out var id) ? id.GetString() : null;
        var nome = curso.TryGetProperty("nome", out var n) ? n.GetString() : null;
        var valor = curso.TryGetProperty("valor", out var v) ? v.GetDouble() : 0d;
        return (cursoId, nome, valor);
    }

    /// <summary>Consulta {alunoId}/PorId e retorna (matriculaId, status) da matrícula do curso informado.</summary>
    private async Task<(string? matriculaId, int status)> ObterMatriculaAsync(string alunoId, string cursoId)
    {
        var (resposta, corpo) = await EnviarAsync(_fixture.Aluno, HttpMethod.Get, $"/api/Aluno/{alunoId}/PorId", token: null, corpo: null);

        // Protege o polling: resposta sem sucesso ou corpo vazio/não-JSON é tratado como "não encontrado"
        // (retorna -1), para que AguardarStatusAsync continue tentando até o timeout em vez de lançar exceção.
        if (!resposta.IsSuccessStatusCode || string.IsNullOrWhiteSpace(corpo))
        {
            return (null, -1);
        }

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(corpo);
        }
        catch (JsonException)
        {
            return (null, -1);
        }

        using (doc)
        {
            if (!doc.RootElement.TryGetProperty("result", out var result)
                || !result.TryGetProperty("matriculas", out var matriculas)
                || matriculas.ValueKind != JsonValueKind.Array)
            {
                return (null, -1);
            }

            foreach (var matricula in matriculas.EnumerateArray())
            {
                var idCurso = matricula.TryGetProperty("cursoId", out var mc) ? mc.GetString() : null;
                if (!string.Equals(idCurso, cursoId, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var id = matricula.TryGetProperty("id", out var mid) ? mid.GetString() : null;
                return (id, LerStatus(matricula));
            }

            return (null, -1);
        }
    }

    /// <summary>Faz polling do status da matrícula até atingir o alvo ou estourar o timeout de 60s.</summary>
    private async Task<int> AguardarStatusAsync(string alunoId, string cursoId, int statusAlvo)
    {
        var limite = DateTime.UtcNow.AddSeconds(TimeoutPollingSegundos);
        var statusAtual = -1;

        while (DateTime.UtcNow < limite)
        {
            (_, statusAtual) = await ObterMatriculaAsync(alunoId, cursoId);
            _saida.WriteLine($"[7-Polling] status atual = {statusAtual} ({DescreverStatus(statusAtual)})");

            if (statusAtual == statusAlvo || statusAtual == StatusPagamentoRecusado)
            {
                break;
            }

            await Task.Delay(TimeSpan.FromSeconds(IntervaloPollingSegundos));
        }

        return statusAtual;
    }

    /// <summary>
    /// Lê o enum StatusMatricula de forma tolerante: aceita número OU string
    /// (PendentePagamento=1, PagamentoRealizado=2, Concluido=3, Cancelada=4, PagamentoRecusado=5).
    /// </summary>
    private static int LerStatus(JsonElement matricula)
    {
        if (!matricula.TryGetProperty("status", out var status))
        {
            return -1;
        }

        if (status.ValueKind == JsonValueKind.Number)
        {
            return status.GetInt32();
        }

        return status.GetString() switch
        {
            "PendentePagamento" => StatusPendentePagamento,
            "PagamentoRealizado" => StatusPagamentoRealizado,
            "Concluido" => 3,
            "Cancelada" => 4,
            "PagamentoRecusado" => StatusPagamentoRecusado,
            _ => -1
        };
    }

    private static string DescreverStatus(int status) => status switch
    {
        StatusPendentePagamento => "PendentePagamento",
        StatusPagamentoRealizado => "PagamentoRealizado",
        3 => "Concluido",
        4 => "Cancelada",
        StatusPagamentoRecusado => "PagamentoRecusado",
        _ => "Desconhecido"
    };
}
