using FluentValidation;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Messages.AlunoCommands;


namespace MBA.Aluno.Appplication.Commands.CadastroAluno
{
    public class CadastroAlunoCommandValidator : AbstractValidator<CadastroAlunoCommand>
    {
        public CadastroAlunoCommandValidator()
        {
            RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno inválido.");
            RuleFor(c => c.Nome).NotEmpty().WithMessage("O nome é obrigatório.");
            RuleFor(c => c.Email).NotEmpty().WithMessage("O email é obrigatório.").EmailAddress().WithMessage("Email inválido.");
        }
    }
}
