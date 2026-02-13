using MBA.Core.DomainValidations;
using MBA.Pagamentos.Domain.Enumerators;

namespace MBA.Pagamentos.Domain.ValueObjects;
public class StatusPagamento
{
    #region Atributos
    public StatusPagamentoEnum Status { get; private set; }

    protected StatusPagamento() { }

    public StatusPagamento(StatusPagamentoEnum status)
    {
        Status = status;
    }
    #endregion

    #region Métodos
    public bool EstahPendente => Status == StatusPagamentoEnum.Pendente;
    public bool EstahAprovado => Status == StatusPagamentoEnum.Aprovado;
    public bool EstahRecusado => Status == StatusPagamentoEnum.Recusado;

    public void TransicionarPara(StatusPagamentoEnum novoStatus)
    {
        ValidarIntegridadeStatusPagamento(novoStatus);
        Status = novoStatus;
    }

    private void ValidarIntegridadeStatusPagamento(StatusPagamentoEnum status)
    {
        var validacao = new ResultadoValidacao<StatusPagamento>();

        if (Status == StatusPagamentoEnum.Aprovado) { validacao.AdicionarErro("Pagamento aprovado não pode ser alterado."); }
        if (Status == StatusPagamentoEnum.Recusado) { validacao.AdicionarErro("Pagamento recusado não pode ser alterado."); }
        if (Status == StatusPagamentoEnum.Pendente && status == StatusPagamentoEnum.Pendente) { validacao.AdicionarErro("Pagamento já está pendente."); }

        validacao.DispararExcecaoDominioSeInvalido();
    }

    public override string ToString() => Status.ToString();
    #endregion
}
