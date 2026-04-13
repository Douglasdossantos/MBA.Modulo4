namespace MBA.Core.DomainObjects;

public class DomainException : Exception
{
	public IReadOnlyCollection<string> Errors { get; }

	public DomainException(string message) : base(message)
	{
		Errors = [message];
	}

	public DomainException(IEnumerable<string> mensagens)
	{
		var list = mensagens.ToList();
		Errors = list.AsReadOnly();
	}
}