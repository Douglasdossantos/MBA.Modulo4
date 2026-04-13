using MBA.Bff.Api.Models.Autenticacao;
using System.ComponentModel.DataAnnotations;

namespace MBA.Bff.Api.Models.Aluno;

public class AulaAssistidaViewModel
{
	public UsuarioLoginViewModel Login { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public Guid AlunoId { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public Guid MatriculaId { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public Guid AulaId { get; set; }
}