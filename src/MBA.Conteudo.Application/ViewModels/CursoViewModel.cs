using System.ComponentModel.DataAnnotations;
using MBA.Conteudo.Domain.Entities;

namespace MBA.Conteudo.Application.ViewModels;

public class CursoViewModel
{
	[Key] public Guid Id { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public bool Ativo { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public string Nome { get; set; } = string.Empty;

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public DateTime Validade { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public string Finalidade { get; set; } = string.Empty;

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public string Ementa { get; set; } = string.Empty;

	public static implicit operator CursoViewModel(Curso curso)
	{
		return new CursoViewModel
		{
			Id = curso.Id,
			Ativo = curso.Ativo,
			Nome = curso.Nome,
			Validade = curso.ValidoAte ?? DateTime.MinValue,
			Finalidade = curso.ConteudoProgramatico.Finalidade,
			Ementa = curso.ConteudoProgramatico.Ementa
		};
	}
}