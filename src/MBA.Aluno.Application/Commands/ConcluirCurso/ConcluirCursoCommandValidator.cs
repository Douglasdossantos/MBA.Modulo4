using FluentValidation;
using MBA.Core.Messages.AlunoCommands;

namespace MBA.Aluno.Application.Commands.ConcluirCurso;

internal class ConcluirCursoCommandValidator : AbstractValidator<ConcluirCursoCommand>
{
	public ConcluirCursoCommandValidator()
	{
		RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno inválido.");
		RuleFor(c => c.MatriculaId).NotEqual(Guid.Empty).WithMessage("Id da matricula inválido.");
	}
}