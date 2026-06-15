using System.ComponentModel.DataAnnotations;

namespace MBA.Aluno.Application.ViewModel;

public class AulaAssistidaViewModel
{
	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public Guid AlunoId { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public Guid MatriculaId { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public Guid AulaId { get; set; }
}