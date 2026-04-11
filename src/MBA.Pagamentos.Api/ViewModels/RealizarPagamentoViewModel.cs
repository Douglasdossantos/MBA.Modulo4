namespace MBA.Pagamentos.Api.ViewModels;

public class RealizarPagamentoViewModel
{
	public Guid AlunoId { get; set; }
	public Guid CursoId { get; set; }
	public Guid MatriculaCursoId { get; set; }
	public bool PagamentoPodeSerRealizado { get; set; }
	public string NomeCurso { get; set; } = string.Empty;
	public DateTime DataMatricula { get; set; } = new();
	public DateTime? DataConclusao { get; set; }
	public string EstadoMatricula { get; set; } = string.Empty;
	public decimal Valor { get; set; }
	public string NumeroCartao { get; set; } = string.Empty;
	public string NomeTitularCartao { get; set; } = string.Empty;
	public string ValidadeCartao { get; set; } = string.Empty;
	public string CvvCartao { get; set; } = string.Empty;
}