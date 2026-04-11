using FluentValidation.Results;

namespace MBA.Core.Messages.Integration;

public class ResponseMessage(ValidationResult validationResult) : Message
{
	public ValidationResult ValidationResult { get; set; } = validationResult;
}