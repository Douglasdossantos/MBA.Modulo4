using MBA.Aluno.Domain.Entities;
using MBA.Core.SharedDto.Aluno.Enum;

using System.ComponentModel.DataAnnotations;

namespace MBA.Aluno.Application.ViewModel;

public class MatriculaViewModel
{
	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public Guid CursoId { get; set; }

	[Required(ErrorMessage = "O campo {0} é obrigatório")]
	public Guid AlunoId { get; set; }

	public static implicit operator Matricula(MatriculaViewModel vm)
	{
		return new Matricula(
			vm.CursoId,
			vm.AlunoId,
			DateTime.Now,
			StatusMatricula.PendentePagamento);
	}

	public static implicit operator MatriculaViewModel(Matricula matricula)
	{
		return new MatriculaViewModel
		{
			CursoId = matricula.CursoId,
			AlunoId = matricula.AlunoId
		};
	}
}