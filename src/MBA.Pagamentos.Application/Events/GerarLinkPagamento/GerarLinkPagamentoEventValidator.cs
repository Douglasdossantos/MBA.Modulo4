using FluentValidation;
using MBA.Core.Messages.FaturamentoEvents;

namespace MBA.Application.Events.GerarLinkPagamento;
public class GerarLinkPagamentoEventValidator : AbstractValidator<GerarLinkPagamentoEvent>
{
    public GerarLinkPagamentoEventValidator()
    {
        RuleFor(c => c.MatriculaCursoId).NotEqual(Guid.Empty).WithMessage("Id da matrícula é inválida");
        RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno é inválido");
        RuleFor(c => c.CursoId).NotEqual(Guid.Empty).WithMessage("Id do curso é inválido");
        RuleFor(c => c.Valor).GreaterThan(0.00m).WithMessage("Valor tem que ser MAIOR que zero");
    }
}
