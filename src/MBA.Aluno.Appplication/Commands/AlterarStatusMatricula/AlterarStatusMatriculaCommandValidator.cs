using FluentValidation;
using MBA.Core.Messages.AlunoCommands;
using MBA.Core.SharedDto.Aluno.Enum;

namespace MBA.Aluno.Appplication.Commands.AlterarStatusMatricula
{
    internal class AlterarStatusMatriculaCommandValidator : AbstractValidator<AlterarStatusMatriculaCommand>
    {
        public AlterarStatusMatriculaCommandValidator()
        {
            RuleFor(c => c.MatriculaId).NotEqual(Guid.Empty).WithMessage("Id da matricula inválida.");
            RuleFor(c => c.Status).Must(status => Enum.IsDefined(typeof(StatusMatricula), status)).WithMessage("Status inválido.");
        }
    }
}
