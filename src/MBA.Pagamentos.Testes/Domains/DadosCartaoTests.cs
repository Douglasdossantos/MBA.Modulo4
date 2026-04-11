using FluentAssertions;

using MBA.Core.DomainObjects;
using MBA.Pagamentos.Domain.ValueObjects;

namespace MBA.Pagamentos.Testes.Domains;

public class DadosCartaoTests
{
	#region Helpers

	private const string NumeroValido = "5493813493490144";
	private const string NomeValido = "Jairo Azevedo";
	private const string ValidadeValida = "12/96";
	private const string CvvValido = "593";

	private static DadosCartao CriarCartao(string numero = NumeroValido,
		string nome = NomeValido,
		string validade = ValidadeValida,
		string cvv = CvvValido)
	{
		return new DadosCartao(numero, nome, validade, cvv);
	}

	#endregion

	#region Construtores

	[Fact]
	public void Deve_criar_dados_cartao_validos()
	{
		var cartao = CriarCartao();

		cartao.Should().NotBeNull();
		cartao.Numero.Should().Be(NumeroValido);
		cartao.NomeTitular.Should().Be(NomeValido);
		cartao.Validade.Should().Be(ValidadeValida);
		cartao.Cvv.Should().Be(CvvValido);
	}

	[Theory]
	[InlineData(null, NomeValido, ValidadeValida, CvvValido, "*Número do cartão deve ser informado*")]
	[InlineData("123", NomeValido, ValidadeValida, CvvValido, "*Número do cartão deve possuir 16 caracteres*")]
	[InlineData("12345678901234567", NomeValido, ValidadeValida, CvvValido,
		"*Número do cartão deve possuir 16 caracteres*")]
	[InlineData(NumeroValido, null, ValidadeValida, CvvValido, "*Nome do titular deve ser informado*")]
	[InlineData(NumeroValido, "ab", ValidadeValida, CvvValido, "*Nome do titular deve ter entre 3 e 50 caracteres*")]
	[InlineData(NumeroValido, NomeValido, null, CvvValido, "*Validade do cartão deve ser informada*")]
	[InlineData(NumeroValido, NomeValido, "13/29", CvvValido, "*Validade do cartão deve estar no formato MM/AA*")]
	[InlineData(NumeroValido, NomeValido, ValidadeValida, null, "*CVV deve ser informado*")]
	[InlineData(NumeroValido, NomeValido, ValidadeValida, "12", "*CVV deve possuir 3 caracteres*")]
	[InlineData(NumeroValido, NomeValido, ValidadeValida, "1283", "*CVV deve possuir 3 caracteres*")]
	[InlineData(NumeroValido, NomeValido, ValidadeValida, "abc", "*CVV deve conter apenas números*")]
	public void Nao_deve_criar_dados_cartao_invalidos(string numero, string nome, string validade, string cvv,
		string mensagemEsperada)
	{
		Action act = () => CriarCartao(numero,
			nome,
			validade,
			cvv);

		act.Should().Throw<DomainException>()
			.WithMessage(mensagemEsperada);
	}

	#endregion

	#region Metodos do Dominio

	#endregion

	#region Overrides

	[Fact]
	public void ToString_deve_conter_nome_e_validade()
	{
		var cartao = CriarCartao();
		cartao.ToString().Should().Contain(NomeValido)
			.And.Contain(ValidadeValida);
	}

	#endregion
}