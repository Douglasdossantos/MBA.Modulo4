using FluentValidation;
using MBA.Core.Messages.AlunoCommands;

namespace MBA.Aluno.Application.Commands.CadastroAluno;

public class CadastroAlunoCommandValidator : AbstractValidator<CadastroAlunoCommand>
{
	public CadastroAlunoCommandValidator()
	{
		RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno inválido.");
		RuleFor(c => c.Nome).NotEmpty().WithMessage("O nome é obrigatório.");
		RuleFor(c => c.Email).NotEmpty().WithMessage("O email é obrigatório.").EmailAddress()
			.WithMessage("Email inválido.");
	}
}