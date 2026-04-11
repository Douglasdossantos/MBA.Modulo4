using MBA.Bff.Api.Models.Autenticacao;
using System.ComponentModel.DataAnnotations;

namespace MBA.Bff.Api.Models.Aluno;

public class MatriculaViewModel
{
	public UsuarioLoginViewModel Login { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public Guid CursoId { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public Guid AlunoId { get; set; }

	public Guid MatriculaCursoId { get; set; }
	public bool PagamentoPodeSerRealizado { get; set; }
	public string NomeCurso { get; set; } = string.Empty;
	public DateTime DataMatricula { get; set; }
	public DateTime? DataConclusao { get; set; }
	public string EstadoMatricula { get; set; } = string.Empty;
	public decimal Valor { get; set; }
	public string NumeroCartao { get; set; } = string.Empty;
	public string NomeTitularCartao { get; set; } = string.Empty;
	public string ValidadeCartao { get; set; } = string.Empty;
	public string CvvCartao { get; set; } = string.Empty;
}