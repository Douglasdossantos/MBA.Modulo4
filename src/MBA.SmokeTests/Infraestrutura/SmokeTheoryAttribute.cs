using MBA.SmokeTests.Configuracao;

namespace MBA.SmokeTests.Infraestrutura;

/// <summary>
/// Variação de <see cref="TheoryAttribute"/> que só executa quando a variável de ambiente
/// EXECUTAR_SMOKE_TESTS=true. Caso contrário, marca o teste como Skipped, protegendo o CI existente.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class SmokeTheoryAttribute : TheoryAttribute
{
    public SmokeTheoryAttribute()
    {
        if (!ConfiguracaoSmoke.SmokeHabilitado)
        {
            Skip = ConfiguracaoSmoke.MensagemSkip;
        }
    }
}
