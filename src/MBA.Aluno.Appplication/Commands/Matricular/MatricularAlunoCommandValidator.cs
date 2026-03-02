using FluentValidation;
using MBA.Core.Messages.AlunoCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Appplication.Commands.Matricular
{
    public class MatricularAlunoCommandValidator : AbstractValidator<MatricularAlunoCommand>
    {
        public MatricularAlunoCommandValidator()
        {
            RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno inválido.");
            RuleFor(c => c.CursoId).NotEqual(Guid.Empty).WithMessage("Id do Curso inválido.");
        }

    }
}
