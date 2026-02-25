using FluentValidation;
using MBA.Core.Messages;

namespace MBA.Aluno.API.Application.Commands
{
    public class RegistarAlunoCommand : Command
    {
        public RegistarAlunoCommand(Guid id, DateTime criadoEm)
        {
            AggregateId = id;
            Id = id;
            CriadoEm = criadoEm;
        }

        public Guid Id { get; private set; }
        public DateTime CriadoEm { get; private set; }

        public override bool Valido()
        {
            ValidationResult = new RegistrarAlunoValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
    public class RegistrarAlunoValidation : AbstractValidator<RegistarAlunoCommand>
    {
        public RegistrarAlunoValidation()
        {
            RuleFor(c => c.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do Aluno  Inválido");

            RuleFor(c => c.CriadoEm)
            .NotEmpty()
            .WithMessage("A data de criação é obrigatória.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("A data de criação não pode estar no futuro.");

            RuleFor(c => c.CriadoEm)
                .GreaterThan(DateTime.UtcNow.AddYears(-1))
                .WithMessage("Data de criação inválida.");
        }
    }
}
