using FluentValidation.Results;

using MBA.Core.Data;

namespace MBA.Core.Messages;

public abstract class CommandHandler
{
	protected ValidationResult ValidationResult;

	protected CommandHandler()
	{
		ValidationResult = new ValidationResult();
	}

	protected void AdicionarErro(string messagem)
	{
		ValidationResult.Errors.Add(new ValidationFailure(string.Empty, messagem));
	}

	protected async Task<ValidationResult> PersistirDados(IUnitOfWork uow)
	{
		if (!await uow.Commit()) AdicionarErro("Houve um erro ao persistir os dados");
		return ValidationResult;
	}
}