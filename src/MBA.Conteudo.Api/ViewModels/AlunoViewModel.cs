using MBA.Core.SharedDto.Aluno;

namespace MBA.Conteudo.Api.ViewModels;

public class AlunoViewModel
{
	public Guid Id { get; set; }
	public string Nome { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public DateTime DataNascimento { get; set; }

	public ICollection<MatriculaCursoViewModel> MatriculasCursos { get; set; } = [];

	public static implicit operator AlunoViewModel(AlunoDto dto)
	{
		return new AlunoViewModel
		{
			Id = dto.Id,
			Nome = dto.Nome,
			Email = dto.Email
		};
	}
}