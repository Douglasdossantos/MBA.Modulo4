using FluentValidation;

using MBA.Core.Messages.AlunoCommands;

namespace MBA.Aluno.Application.Commands.RegistrarAulaAssistida;

public class RegistrarAulaAssistidaCommandValidator : AbstractValidator<RegistrarAulaAssistidaCommand>
{
	public RegistrarAulaAssistidaCommandValidator()
	{
		RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno inválido.");
		RuleFor(c => c.MatriculaCursoId).NotEqual(Guid.Empty).WithMessage("Id da matrícula inválido.");
		RuleFor(c => c.AulaId).NotEqual(Guid.Empty).WithMessage("Id da aula inválido.");
	}
}
