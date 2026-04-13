using MBA.Core.SharedDto.Aluno.Enum;

namespace MBA.Core.SharedDto.Aluno;

public class MatriculaStatusDto
{
	public Guid Id { get; set; }
	public Guid AlunoId { get; set; }
	public Guid CursoId { get; set; }
	public string Status { get; set; } = string.Empty;
	public bool PodeSerPaga { get; set; }

	public static MatriculaStatusDto FromMatricula(Guid id, Guid alunoId, Guid cursoId, StatusMatricula status)
	{
		return new MatriculaStatusDto
		{
			Id = id,
			AlunoId = alunoId,
			CursoId = cursoId,
			Status = status.ToString(),
			PodeSerPaga = status == StatusMatricula.PendentePagamento
		};
	}
}
