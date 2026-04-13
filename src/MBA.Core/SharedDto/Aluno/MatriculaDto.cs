using MBA.Core.SharedDto.Aluno.Enum;

namespace MBA.Core.SharedDto.Aluno;

public class MatriculaDto
{
	public Guid Id { get; set; }
	public Guid CursoId { get; set; }
	public Guid AlunoId { get; set; }
	public DateTime DataMatricula { get; set; }
	public DateTime DataCursoConcluido { get; set; }
	public int TotalAulas { get; set; }
	public int AulasAssistidas { get; set; }
	public int AulasFaltantes { get; set; }
	public decimal Porcentagem { get; set; }

	public StatusMatricula Status { get; set; }
	public CertificadoDto Certificado { get; set; } = null!;
}