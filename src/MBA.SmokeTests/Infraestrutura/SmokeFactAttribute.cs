using MBA.SmokeTests.Configuracao;

namespace MBA.SmokeTests.Infraestrutura;

/// <summary>
/// Variação de <see cref="FactAttribute"/> que só executa quando a variável de ambiente
/// EXECUTAR_SMOKE_TESTS=true. Caso contrário, marca o teste como Skipped. Isso é crítico:
/// o CI existente roda `dotnet test MBA.Modulo4.sln` e não pode quebrar por causa dos smoke tests,
/// que dependem de um ambiente docker-compose no ar.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class SmokeFactAttribute : FactAttribute
{
    public SmokeFactAttribute()
    {
        if (!ConfiguracaoSmoke.SmokeHabilitado)
        {
            Skip = ConfiguracaoSmoke.MensagemSkip;
        }
    }
}
