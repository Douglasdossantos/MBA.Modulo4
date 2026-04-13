using FluentValidation.Results;

using MediatR;

namespace MBA.Core.Messages;

public abstract class Command : Message, IRequest<ValidationResult>
{
	protected Command()
	{
		TimeStamp = DateTime.Now;
	}

	public DateTime TimeStamp { get; private set; }
	public ValidationResult ValidationResult { get; set; } = new();

	public Guid RaizAgregacao { get; internal set; }

	public virtual bool Valido()
	{
		throw new NotImplementedException();
	}

	public void DefinirRaizAgregacao(Guid raizAgregacao)
	{
		RaizAgregacao = raizAgregacao;
	}

	public void DefinirValidacao(ValidationResult validacao)
	{
		ValidationResult = validacao;
	}

	public ICollection<string> Erros =>
		ValidationResult.Errors.Select(e => e.ErrorMessage).ToList();

	public virtual bool EhValido()
	{
		return ValidationResult.IsValid;
	}
}