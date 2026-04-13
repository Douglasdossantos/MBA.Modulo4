using MBA.Core.SharedDto;

namespace MBA.Conteudo.Api.ViewModels;

public class EvolucaoAlunoDto
{
	public Guid Id { get; set; }
	public string Nome { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public DateTime DataNascimento { get; set; }

	public ICollection<EvolucaoMatriculaCursoDto> MatriculasCursos { get; set; } = [];
}