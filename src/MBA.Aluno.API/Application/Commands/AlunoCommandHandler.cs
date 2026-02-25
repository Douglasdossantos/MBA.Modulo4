using FluentValidation.Results;
using MBA.Aluno.API.Application.Events;
using MBA.Aluno.API.Models;
using MBA.Core.Messages;
using MediatR;

namespace MBA.Aluno.API.Application.Commands
{
    public class AlunoCommandHandler: CommandHandler, IRequestHandler<RegistarAlunoCommand, ValidationResult>
    {
        private readonly IAlunoRepository _repository;

        public AlunoCommandHandler(IAlunoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ValidationResult> Handle(RegistarAlunoCommand message, CancellationToken cancellationToken)
        {
            if (!message.Valido())
            {
                return message.ValidationResult;
            }

            var aluno = new Models.Aluno(message.Id, message.CriadoEm);

            var alunoExistente =  await _repository.ObterAlunoId(message.Id);

            if (alunoExistente != null)
            {
                AdicionarErro("Esse Id ja está sendo usado/incorreto");
                return ValidationResult;
            }



            _repository.AdicionarAluno(aluno);

            aluno.AdicionarEvento(new AlunoRegistradoEvent(message.Id,message.CriadoEm));

            return await PersistirDados(_repository.UnitOfWork);
        }
    }
}
