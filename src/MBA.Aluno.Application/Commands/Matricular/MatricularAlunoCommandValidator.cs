using FluentValidation;
using MBA.Core.Messages.AlunoCommands;

namespace MBA.Aluno.Application.Commands.Matricular;

public class MatricularAlunoCommandValidator : AbstractValidator<MatricularAlunoCommand>
{
	public MatricularAlunoCommandValidator()
	{
		RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno inválido.");
		RuleFor(c => c.CursoId).NotEqual(Guid.Empty).WithMessage("Id do Curso inválido.");
	}
}