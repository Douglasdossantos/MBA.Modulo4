namespace MBA.Pagamentos.Application.Queries.Dtos;

public class PagamentoStatusDto
{
    public Guid Id { get; set; }
    public Guid MatriculaCursoId { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? DataPagamento { get; set; }
    public string? TransacaoId { get; set; }
}
