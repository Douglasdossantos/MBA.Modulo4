namespace MBA.Bff.Api.Models.Aluno;

public class AlunoViewModel
{
	public Guid Id { get; set; }
	public string Email { get; set; }
	public string Nome { get; set; }
	public bool Ativo { get; set; }
	public bool Adm { get; set; }
	public DateTime DataCriacao { get; set; }
}