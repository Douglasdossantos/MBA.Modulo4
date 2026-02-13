using FluentValidation;
using MBA.Messages.FaturamentoCommands;



namespace SaberOnline.Application.Application.Commands.RealizarPagamento;
public class RealizarPagamentoCommandValidator : AbstractValidator<RealizarPagamentoCommand>
{
    public RealizarPagamentoCommandValidator()
    {
        RuleFor(c => c.MatriculaCursoId).NotEqual(Guid.Empty).WithMessage("Id da matrícula é inválida");
        RuleFor(c => c.MatriculaCursoDto).NotNull().WithMessage("Matricula Curso deve ser informado");
        RuleFor(c => c.Valor).GreaterThan(0).WithMessage("Valor de pagamento é inválido");
        RuleFor(c => c.NumeroCartao).NotEmpty().Length(16).WithMessage("Número do cartão não reconhecido");
        RuleFor(c => c.NomeTitularCartao).NotEmpty().Length(3, 50).WithMessage("Nome do titular é inválido");
        RuleFor(c => c.ValidadeCartao).NotEmpty().Matches("^(0[1-9]|1[0-2])\\/\\d{2}$").WithMessage("Validade do cartão é inválida");
        RuleFor(c => c.CvvCartao).NotEmpty().Length(3).Matches("^[0-9]{3}$").WithMessage("Informe um CVV válido");
    }
}
