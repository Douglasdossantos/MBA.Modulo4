namespace MBA.Core.SharedDto;
public class AulaCursoViewModel
{
    public Guid AulaId { get; set; }
    public Guid CursoId { get; set; }
    public string NomeAula { get; set; } = string.Empty;
    public byte OrdemAula { get; set; }
    public bool Ativo { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataTermino { get; set; }
    public bool AulaJaIniciadaRealizada => DataTermino.HasValue;
    public string Url { get; set; } = string.Empty;

    public static implicit operator AulaCursoViewModel(AulaCursoDto dto) => new()
    {
        AulaId = dto.AulaId,
        CursoId = dto.CursoId,
        NomeAula = dto.NomeAula,
        OrdemAula = dto.OrdemAula,
        Ativo = dto.Ativo,
        DataInicio = dto.DataInicio,
        DataTermino = dto.DataTermino,
        Url = dto.Url
    };
}
