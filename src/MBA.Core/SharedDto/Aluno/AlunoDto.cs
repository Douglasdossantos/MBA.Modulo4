namespace MBA.Core.SharedDto.Aluno;

public class AlunoDto
{
	public Guid Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string Nome { get; set; } = string.Empty;
	public bool Ativo { get; set; }
	public bool Adm { get; set; }

	public DateTime DataCriacao { get; set; }

	public ICollection<MatriculaDto> Matriculas { get; set; } = [];
}