namespace MBA.Core.SharedDto;
public class AulaCursoDto
{
    public Guid AulaId { get; set; }
    public Guid CursoId { get; set; }
    public string NomeAula { get; set; }
    public byte OrdemAula { get; set; }
    public bool Ativo { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataTermino { get; set; }
    public bool AulaJaIniciadaRealizada => DataTermino.HasValue;
    public string Url { get; set; }
}
