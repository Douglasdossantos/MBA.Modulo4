namespace MBA.API.ViewModels;
public class EvolucaoMatriculaCursoViewModel
{
    public Guid Id { get; set; }
    public Guid CursoId { get; set; }
    public string NomeCurso { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataMatricula { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string EstadoMatricula { get; set; }
    public CertificadoViewModel Certificado { get; set; }
    public int QuantidadeAulasNoCurso { get; set; }
    public int QuantidadeAulasRealizadas { get; set; }
    public int QuantidadeAulasEmAndamento { get; set; }
}
