using Microsoft.Extensions.Configuration;

namespace MBA.WebApi.Core.Extensions;

/// <summary>
/// Validação "fail-fast" de segredos obrigatórios. Se as chaves esperadas (que hoje vêm do
/// Infisical) não estiverem presentes, a aplicação NÃO sobe e exibe uma mensagem clara com as
/// opções de correção — em vez de estourar depois com um erro críptico de conexão/null.
/// </summary>
public static class ValidacaoConfiguracaoExtension
{
	public static void ValidarSegredosObrigatorios(this IConfiguration configuration, params string[] chaves)
	{
		var faltando = chaves
			.Where(chave => string.IsNullOrWhiteSpace(configuration[chave]))
			.ToArray();

		if (faltando.Length == 0) return;

		var mensagem =
			"\n" +
			"==================================================================================\n" +
			"  CONFIGURACAO AUSENTE — A APLICACAO NAO PODE INICIAR\n" +
			"==================================================================================\n" +
			$"  Segredos faltando: {string.Join(", ", faltando)}\n" +
			"\n" +
			"  Estes valores vem do INFISICAL. Voce precisa de UMA das opcoes abaixo:\n" +
			"\n" +
			"  --> OPCAO 1 (recomendada): usar o INFISICAL CLI\n" +
			"      1. Instale:  winget install infisical\n" +
			"      2. Faca login: infisical login --domain=https://infisical.dots.dev.br\n" +
			"      3. No Visual Studio, selecione o profile \"Infisical (dev)\" e rode (F5).\n" +
			"         (ou no terminal: infisical run --env=dev -- dotnet run --project <projeto>)\n" +
			"\n" +
			"  --> OPCAO 2: configurar MANUALMENTE (sem Infisical), via user-secrets:\n" +
			string.Join("\n", faltando.Select(k => $"      dotnet user-secrets set \"{k}\" \"<valor>\"")) + "\n" +
			"      (ou preencha as mesmas chaves no appsettings.Development.json)\n" +
			"==================================================================================\n";

		throw new InvalidOperationException(mensagem);
	}
}
