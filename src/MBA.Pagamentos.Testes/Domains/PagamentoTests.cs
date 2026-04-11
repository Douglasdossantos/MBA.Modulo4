using FluentAssertions;
using MBA.Core.DomainObjects;
using MBA.Pagamentos.Domain.Entities;
using MBA.Pagamentos.Domain.Enumerators;
using MBA.Pagamentos.Domain.ValueObjects;


namespace MBA.Pagamentos.Testes.Domains;

public class PagamentoTests
{
	#region Helpers

	private const string MatriculaIdValido = "11111111-1111-1111-1111-111111111111";
	private static readonly DateTime DataVencimentoFutura = DateTime.Now.AddDays(7);
	private const double ValorValido = 1000.00;

	private const string NumeroValido = "5493813493490144";
	private const string NomeValido = "Jairo Azevedo";
	private const string ValidadeValida = "12/96";
	private const string CvvValido = "593";

	private static DadosCartao CartaoValido => new(NumeroValido,
		NomeValido,
		ValidadeValida,
		CvvValido);

	private static Pagamento CriarPagamento(string matriculaId = MatriculaIdValido,
		double valor = ValorValido,
		DateTime? vencimento = null)
	{
		return new Pagamento(Guid.Parse(matriculaId), (decimal)valor, vencimento ?? DataVencimentoFutura);
	}

	#endregion

	#region Construtores

	[Fact]
	public void Deve_criar_pagamento_valido()
	{
		var pagamento = CriarPagamento();

		pagamento.Should().NotBeNull();
		pagamento.MatriculaId.Should().Be(Guid.Parse(MatriculaIdValido));
		pagamento.Valor.Should().Be((decimal)ValorValido);
		pagamento.DataVencimento.Date.Should().Be(DataVencimentoFutura.Date);
		pagamento.Cartao.Should().BeNull();
		pagamento.StatusPagamento.Status.Should().Be(StatusPagamentoEnum.Pendente);
		pagamento.DataPagamento.Should().BeNull();
	}

	[Theory]
	[InlineData("00000000-0000-0000-0000-000000000000", ValorValido, "*Matrícula do curso não foi informada*")]
	[InlineData(MatriculaIdValido, 0.0, "*Valor do pagamento deve ser maior que zero*")]
	public void Nao_deve_criar_pagamento_invalido(string matriculaId, double valor, string mensagemEsperada)
	{
		Action act = () => CriarPagamento(matriculaId, valor);

		act.Should().Throw<DomainException>()
			.WithMessage(mensagemEsperada);
	}

	#endregion

	#region Métodos de Pagamento

	[Fact]
	public void Deve_confirmar_pagamento()
	{
		var pagamento = CriarPagamento();

		var dadosCartao = new DadosCartao(NumeroValido, NomeValido, ValidadeValida, CvvValido);
		pagamento.ConfirmarPagamento(null, "uiouoiuoiu", dadosCartao);

		pagamento.StatusPagamento.Status.Should().Be(StatusPagamentoEnum.Aprovado);
		pagamento.DataPagamento.Should().NotBeNull();
		pagamento.PossuiPagamentoAprovado().Should().BeTrue();
	}

	[Fact]
	public void Deve_recusar_pagamento()
	{
		var pagamento = CriarPagamento();

		pagamento.RecusarPagamento();

		pagamento.StatusPagamento.Status.Should().Be(StatusPagamentoEnum.Recusado);
		pagamento.DataPagamento.Should().BeNull();
	}

	[Fact]
	public void Nao_deve_confirmar_pagamento_com_codigo_invalido()
	{
		var pagamento = CriarPagamento();
		var cartao = CartaoValido;

		var act = () => pagamento.ConfirmarPagamento(null, "", cartao);

		act.Should().Throw<DomainException>()
			.WithMessage("*Código de confirmação do pagamento deve ser informado*");
	}

	#endregion
}