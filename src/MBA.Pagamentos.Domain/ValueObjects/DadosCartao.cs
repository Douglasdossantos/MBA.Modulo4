using MBA.Core.DomainValidations;

namespace MBA.Pagamentos.Domain.ValueObjects;

public class DadosCartao
{
	#region Atributods

	public string Numero { get; private set; } = string.Empty;
	public string NomeTitular { get; private set; } = string.Empty;
	public string Validade { get; private set; } = string.Empty;
	public string Cvv { get; private set; } = string.Empty;

	protected DadosCartao() { }

	public DadosCartao(string numero, string nomeTitular, string validade, string cvv)
	{
		Numero = numero;
		NomeTitular = nomeTitular;
		Validade = validade;
		Cvv = cvv;

		ValidarIntegridadeDadosCartao();
	}

	#endregion

	#region Metodos

	private void ValidarIntegridadeDadosCartao()
	{
		var validacao = new ResultadoValidacao<DadosCartao>();
		ValidacaoTexto.DevePossuirConteudo(Numero, "Número do cartão deve ser informado", validacao);
		ValidacaoTexto.DevePossuirTamanho(Numero, 16, 16, "Número do cartão deve possuir 16 caracteres", validacao);
		ValidacaoTexto.DevePossuirConteudo(NomeTitular, "Nome do titular deve ser informado", validacao);
		ValidacaoTexto.DevePossuirTamanho(NomeTitular, 3, 50, "Nome do titular deve ter entre 3 e 50 caracteres",
			validacao);
		ValidacaoTexto.DevePossuirConteudo(Validade, "Validade do cartão deve ser informada", validacao);
		ValidacaoTexto.DeveAtenderRegex(Validade, @"^(0[1-9]|1[0-2])\/\d{2}$",
			"Validade do cartão deve estar no formato MM/AA", validacao);
		ValidacaoTexto.DevePossuirConteudo(Cvv, "CVV deve ser informado", validacao);
		ValidacaoTexto.DevePossuirTamanho(Cvv, 3, 3, "CVV deve possuir 3 caracteres", validacao);
		ValidacaoTexto.DeveAtenderRegex(Cvv, @"^\d{3}$", "CVV deve conter apenas números", validacao);

		validacao.DispararExcecaoDominioSeInvalido();
	}

	public override string ToString()
	{
		return $"Dados do cartão: {NomeTitular}, validade: {Validade}";
	}

	#endregion
}