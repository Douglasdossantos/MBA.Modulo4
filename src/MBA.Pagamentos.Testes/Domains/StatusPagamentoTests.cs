using FluentAssertions;
using MBA.Core.DomainObjects;
using MBA.Pagamentos.Domain.Enumerators;
using MBA.Pagamentos.Domain.ValueObjects;



namespace MBA.Pagamentos.Testes.Domains;
public class StatusPagamentoTests
{
    #region Helpers
    private StatusPagamento CriarStatusPagamento(StatusPagamentoEnum status)
    {
        return new StatusPagamento(status);
    }
    #endregion

    #region Transições Válidas
    [Theory]
    [InlineData(StatusPagamentoEnum.Pendente, StatusPagamentoEnum.Aprovado)]
    [InlineData(StatusPagamentoEnum.Pendente, StatusPagamentoEnum.Recusado)]
    public void Deve_transicionar_status_valido(StatusPagamentoEnum atual, StatusPagamentoEnum novo)
    {
        var status = CriarStatusPagamento(atual);

        status.TransicionarPara(novo);

        status.Status.Should().Be(novo);
    }
    #endregion

    #region Transições Inválidas
    [Theory]
    [InlineData(StatusPagamentoEnum.Aprovado, StatusPagamentoEnum.Pendente, "*Pagamento aprovado não pode ser alterado*")]
    [InlineData(StatusPagamentoEnum.Aprovado, StatusPagamentoEnum.Recusado, "*Pagamento aprovado não pode ser alterado*")]
    [InlineData(StatusPagamentoEnum.Recusado, StatusPagamentoEnum.Aprovado, "*Pagamento recusado não pode ser alterado*")]
    [InlineData(StatusPagamentoEnum.Recusado, StatusPagamentoEnum.Pendente, "*Pagamento recusado não pode ser alterado*")]
    [InlineData(StatusPagamentoEnum.Pendente, StatusPagamentoEnum.Pendente, "*Pagamento já está pendente*")]
    public void Nao_deve_transicionar_status_invalido(StatusPagamentoEnum atual, StatusPagamentoEnum novo, string mensagemEsperada)
    {
        var status = CriarStatusPagamento(atual);

        Action act = () => status.TransicionarPara(novo);

        act.Should().Throw<DomainException>()
           .WithMessage(mensagemEsperada);
    }

    [Theory]
    [InlineData(StatusPagamentoEnum.Pendente, true, false, false)]
    [InlineData(StatusPagamentoEnum.Aprovado, false, true, false)]
    [InlineData(StatusPagamentoEnum.Recusado, false, false, true)]
    public void Deve_retornar_estado_correto(StatusPagamentoEnum status, bool pendente, bool aprovado, bool recusado)
    {
        var s = new StatusPagamento(status);

        s.EstahPendente.Should().Be(pendente);
        s.EstahAprovado.Should().Be(aprovado);
        s.EstahRecusado.Should().Be(recusado);
    }
    #endregion
}
