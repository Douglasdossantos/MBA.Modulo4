using MBA.Core.SharedDto;

namespace MBA.Conteudo.Api.ViewModels;

public class MatriculaCursoViewModel
{
	public Guid Id { get; set; }
	public Guid CursoId { get; set; }
	public string NomeCurso { get; set; } = string.Empty;
	public decimal Valor { get; set; }
	public DateTime DataMatricula { get; set; }
	public DateTime? DataConclusao { get; set; }
	public string EstadoMatricula { get; set; } = string.Empty;
	public CertificadoViewModel Certificado { get; set; } = null!;

	public static implicit operator MatriculaCursoViewModel(MatriculaCursoDto dto)
	{
		return new MatriculaCursoViewModel
		{
			Id = dto.Id,
			CursoId = dto.CursoId,
			NomeCurso = dto.NomeCurso,
			Valor = dto.Valor,
			DataMatricula = dto.DataMatricula,
			DataConclusao = dto.DataConclusao,
			EstadoMatricula = dto.EstadoMatricula,
			Certificado = dto.Certificado
		};
	}
}