namespace MBA.Core.SharedDto;

public class EvolucaoMatriculaCursoDto
{
	public Guid Id { get; set; }
	public Guid CursoId { get; set; }
	public string NomeCurso { get; set; } = string.Empty;
	public decimal Valor { get; set; }
	public DateTime DataMatricula { get; set; }
	public DateTime? DataConclusao { get; set; }
	public string EstadoMatricula { get; set; } = string.Empty;
	public CertificadoDto Certificado { get; set; } = null!;
	public int QuantidadeAulasNoCurso { get; set; }
	public int QuantidadeAulasRealizadas { get; set; }
	public int QuantidadeAulasEmAndamento { get; set; }
}