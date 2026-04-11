namespace MBA.Core.Messages.AlunoCommands;

public class CadastroAlunoCommand : Command
{
	public Guid AlunoId { get; private set; }
	public string Nome { get; private set; }
	public string Email { get; private set; }
	public bool Ativo { get; private set; }
	public bool Adm { get; private set; }
	public DateTime DataCriacao { get; private set; }

	public CadastroAlunoCommand(Guid alunoId, string nome, string email, bool ativo, bool adm, DateTime dataCriacao)
	{
		DefinirRaizAgregacao(alunoId);
		AlunoId = alunoId;
		Nome = nome;
		Email = email;
		Ativo = ativo;
		Adm = adm;
		DataCriacao = dataCriacao;
	}
}