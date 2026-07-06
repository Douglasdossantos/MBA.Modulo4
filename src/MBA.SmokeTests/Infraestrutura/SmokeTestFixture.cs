using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using MBA.SmokeTests.Configuracao;

namespace MBA.SmokeTests.Infraestrutura;

/// <summary>
/// Fixture compartilhada entre os testes. Mantém um <see cref="HttpClient"/> por serviço,
/// todos com Timeout de 15s. Cada requisição ainda deve usar um CancellationToken próprio
/// (ver <see cref="NovaCts"/>) para garantir que nenhuma chamada HTTP de saída fique pendurada.
/// O construtor apenas cria os clients (sem I/O), logo é seguro instanciar mesmo quando
/// todos os testes estão skipados.
/// </summary>
public sealed class SmokeTestFixture : IDisposable
{
    /// <summary>Timeout aplicado a cada <see cref="HttpClient"/>.</summary>
    public static readonly TimeSpan TimeoutHttp = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Opções de serialização JSON: camelCase e tolerância para números vindos como string.
    /// Enums que chegam como número OU string são tratados diretamente na leitura via JsonElement.
    /// </summary>
    public static readonly JsonSerializerOptions JsonOpcoes = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public HttpClient Auth { get; }
    public HttpClient Conteudo { get; }
    public HttpClient Aluno { get; }
    public HttpClient Pagamentos { get; }
    public HttpClient Bff { get; }

    public SmokeTestFixture()
    {
        Auth = Criar(ConfiguracaoSmoke.AuthUrl);
        Conteudo = Criar(ConfiguracaoSmoke.ConteudoUrl);
        Aluno = Criar(ConfiguracaoSmoke.AlunoUrl);
        Pagamentos = Criar(ConfiguracaoSmoke.PagamentosUrl);
        Bff = Criar(ConfiguracaoSmoke.BffUrl);
    }

    /// <summary>Resolve o client HTTP a partir do nome lógico do serviço.</summary>
    public HttpClient PorNome(string servico) => servico switch
    {
        "Auth" => Auth,
        "Conteudo" => Conteudo,
        "Aluno" => Aluno,
        "Pagamentos" => Pagamentos,
        "Bff" => Bff,
        _ => throw new ArgumentOutOfRangeException(nameof(servico), servico, "Serviço desconhecido")
    };

    /// <summary>
    /// Cria um token de cancelamento com prazo por requisição, garantindo que nenhuma
    /// chamada HTTP de saída fique pendurada indefinidamente.
    /// </summary>
    public static CancellationTokenSource NovaCts(int segundos = 15) =>
        new(TimeSpan.FromSeconds(segundos));

    private static HttpClient Criar(string baseUrl)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute),
            Timeout = TimeoutHttp
        };
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    public void Dispose()
    {
        Auth.Dispose();
        Conteudo.Dispose();
        Aluno.Dispose();
        Pagamentos.Dispose();
        Bff.Dispose();
    }
}
