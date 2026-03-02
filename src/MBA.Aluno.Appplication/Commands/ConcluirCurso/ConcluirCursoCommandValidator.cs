using FluentValidation;
using MBA.Aluno.Appplication.Interfaces;
using MBA.Aluno.Domain.Entities;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Mediator;
using MBA.Core.Messages.AlunoCommands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Appplication.Commands.ConcluirCurso
{
    internal class ConcluirCursoCommandValidator : AbstractValidator<ConcluirCursoCommand>
    {
        public ConcluirCursoCommandValidator()
        {
            RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno inválido.");
            RuleFor(c => c.MatriculaId).NotEqual(Guid.Empty).WithMessage("Id da matricula inválido.");
        }
    }
}
