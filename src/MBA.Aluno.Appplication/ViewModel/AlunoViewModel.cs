using System.ComponentModel.DataAnnotations;

namespace MBA.Aluno.Application.ViewModel;

public class AlunoViewModel
{
	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public string? Nome { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public string? Email { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public bool Ativo { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public bool Adm { get; set; }

	public static implicit operator Domain.Entities.Aluno(AlunoViewModel vm)
	{
		return new Domain.Entities.Aluno(
			Guid.NewGuid(),
			vm.Nome ?? string.Empty,
			vm.Email ?? string.Empty,
			vm.Ativo,
			vm.Adm,
			DateTime.Now);
	}

	public static implicit operator AlunoViewModel(Domain.Entities.Aluno aluno)
	{
		return new AlunoViewModel
		{
			Nome = aluno.Nome,
			Email = aluno.Email,
			Ativo = aluno.Ativo,
			Adm = aluno.Adm
		};
	}
}