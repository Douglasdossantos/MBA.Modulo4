
namespace MBA.Core.SharedDto;
public class MatriculaCursoDto
{
    public Guid Id { get; set; }
    public Guid CursoId { get; set; }
    public Guid AlunoId { get; set; }
    public bool PagamentoPodeSerRealizado { get; set; }
    public string NomeCurso { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataMatricula { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string EstadoMatricula { get; set; }
    public CertificadoDto Certificado { get; set; }
}
