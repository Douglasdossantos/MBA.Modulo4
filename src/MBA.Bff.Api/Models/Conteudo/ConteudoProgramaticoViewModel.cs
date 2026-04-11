using System.ComponentModel.DataAnnotations;

namespace MBA.Bff.Api.Models.Conteudo;

public class ConteudoProgramaticoViewModel
{
	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	[StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
	public string Finalidade { get; set; } = string.Empty;

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	[StringLength(2000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
	public string Ementa { get; set; } = string.Empty;
}