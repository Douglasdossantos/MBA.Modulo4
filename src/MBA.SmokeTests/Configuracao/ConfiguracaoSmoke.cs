namespace MBA.SmokeTests.Configuracao;

/// <summary>
/// Centraliza a leitura das variáveis de ambiente que configuram os endereços dos
/// serviços exercitados pelos smoke tests e o gate de execução. Todos os valores têm
/// defaults compatíveis com o docker-compose local (ver dossiê técnico).
/// </summary>
public static class ConfiguracaoSmoke
{
    /// <summary>Variável de ambiente que habilita a execução efetiva dos smoke tests.</summary>
    public const string VariavelGate = "EXECUTAR_SMOKE_TESTS";

    /// <summary>
    /// Indica se os smoke tests devem realmente rodar. Quando falso, os atributos
    /// <see cref="Infraestrutura.SmokeFactAttribute"/> e
    /// <see cref="Infraestrutura.SmokeTheoryAttribute"/> marcam os testes como Skipped.
    /// </summary>
    public static bool SmokeHabilitado =>
        string.Equals(
            Environment.GetEnvironmentVariable(VariavelGate),
            "true",
            StringComparison.OrdinalIgnoreCase);

    /// <summary>Mensagem exibida quando um smoke test é ignorado.</summary>
    public static string MensagemSkip =>
        $"Smoke test desabilitado. Defina {VariavelGate}=true e suba o ambiente " +
        "(docker compose up -d --build) antes de executar. Endereços configuráveis via " +
        "SMOKE_AUTH_URL, SMOKE_CONTEUDO_URL, SMOKE_ALUNO_URL, SMOKE_PAGAMENTOS_URL, SMOKE_BFF_URL.";

    /// <summary>Endereço base da Auth API (default http://localhost:5020).</summary>
    public static string AuthUrl => Ler("SMOKE_AUTH_URL", "http://localhost:5020");

    /// <summary>Endereço base da Conteúdo API (default http://localhost:5137).</summary>
    public static string ConteudoUrl => Ler("SMOKE_CONTEUDO_URL", "http://localhost:5137");

    /// <summary>Endereço base da Aluno API (default http://localhost:5236).</summary>
    public static string AlunoUrl => Ler("SMOKE_ALUNO_URL", "http://localhost:5236");

    /// <summary>Endereço base da Pagamentos API (default http://localhost:5190).</summary>
    public static string PagamentosUrl => Ler("SMOKE_PAGAMENTOS_URL", "http://localhost:5190");

    /// <summary>Endereço base do BFF (default http://localhost:5293).</summary>
    public static string BffUrl => Ler("SMOKE_BFF_URL", "http://localhost:5293");

    private static string Ler(string variavel, string padrao)
    {
        var valor = Environment.GetEnvironmentVariable(variavel);
        return string.IsNullOrWhiteSpace(valor) ? padrao : valor.TrimEnd('/');
    }
}
