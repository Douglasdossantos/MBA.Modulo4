namespace MBA.Conteudo.Api.ViewModels;

public class EvolucaoAlunoViewModel
{
	public Guid Id { get; set; }
	public string Nome { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public DateTime DataNascimento { get; set; }

	public ICollection<EvolucaoMatriculaCursoViewModel> MatriculasCursos { get; set; } = [];

	public static implicit operator EvolucaoAlunoViewModel(EvolucaoAlunoDto dto)
	{
		return new EvolucaoAlunoViewModel
		{
			Id = dto.Id,
			Nome = dto.Nome,
			Email = dto.Email,
			DataNascimento = dto.DataNascimento,
			MatriculasCursos = dto.MatriculasCursos.Select(m => (EvolucaoMatriculaCursoViewModel)m).ToList()
		};
	}
}