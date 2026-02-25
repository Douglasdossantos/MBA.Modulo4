using FluentAssertions;
using MBA.Core.DomainObjects;
using MBA.Pagamentos.Domain.Entities;
using MBA.Pagamentos.Domain.Enumerators;
using MBA.Pagamentos.Domain.ValueObjects;



namespace MBA.Pagamentos.Testes.Domains;
public class PagamentoTests
{
    #region Helpers
    private const string _matriculaIdValido = "11111111-1111-1111-1111-111111111111";
    private static readonly DateTime _dataVencimentoFutura = DateTime.Now.AddDays(7);
    private const double _valorValido = 1000.00;

    private const string _numeroValido = "5493813493490144";
    private const string _nomeValido = "Jairo Azevedo";
    private const string _validadeValida = "12/96";
    private const string _cvvValido = "593";

    private static DadosCartao _cartaoValido => new(numero: _numeroValido,
        nomeTitular: _nomeValido,
        validade: _validadeValida,
        cvv: _cvvValido);

    private static Pagamento CriarPagamento(string matriculaId = _matriculaIdValido,
        double valor = _valorValido,
        DateTime? vencimento = null,
        DadosCartao cartao = null)
    {
        return new Pagamento(Guid.Parse(matriculaId), (decimal)valor, vencimento ?? _dataVencimentoFutura);
    }
    #endregion

    #region Construtores
    [Fact]
    public void Deve_criar_pagamento_valido()
    {
        var pagamento = CriarPagamento();

        pagamento.Should().NotBeNull();
        pagamento.MatriculaId.Should().Be(Guid.Parse(_matriculaIdValido));
        pagamento.Valor.Should().Be((decimal)_valorValido);
        pagamento.DataVencimento.Date.Should().Be(_dataVencimentoFutura.Date);
        pagamento.Cartao.Should().BeNull();
        pagamento.StatusPagamento.Status.Should().Be(StatusPagamentoEnum.Pendente);
        pagamento.DataPagamento.Should().BeNull();
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", _valorValido, "*Matrícula do curso não foi informada*")]
    [InlineData(_matriculaIdValido, 0.0, "*Valor do pagamento deve ser maior que zero*")]
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

        DadosCartao dadosCartao = new DadosCartao(_numeroValido, _nomeValido, _validadeValida, _cvvValido);
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
        var cartao = _cartaoValido;

        Action act = () => pagamento.ConfirmarPagamento(null, "", cartao);

        act.Should().Throw<DomainException>()
           .WithMessage("*Código de confirmação do pagamento deve ser informado*");
    }
    #endregion


}
