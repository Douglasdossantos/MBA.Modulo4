namespace MBA.Core.DomainObjects;

public class DomainException : Exception
{
	public IReadOnlyCollection<string> Errors { get; }

	public DomainException(string message) : base(message)
	{
		Errors = [message];
	}

	public DomainException(IEnumerable<string> mensagens)
		: this(mensagens?.ToList() ?? [])
	{
	}

	private DomainException(List<string> mensagens)
		: base(string.Join(Environment.NewLine, mensagens))
	{
		Errors = mensagens.AsReadOnly();
	}
}