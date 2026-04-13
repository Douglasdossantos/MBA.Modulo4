using MBA.Core.SharedDto;

namespace MBA.Conteudo.Api.ViewModels;

public class EvolucaoMatriculaCursoViewModel
{
	public Guid Id { get; set; }
	public Guid CursoId { get; set; }
	public string NomeCurso { get; set; } = string.Empty;
	public decimal Valor { get; set; }
	public DateTime DataMatricula { get; set; }
	public DateTime? DataConclusao { get; set; }
	public string EstadoMatricula { get; set; } = string.Empty;
	public CertificadoViewModel Certificado { get; set; } = null!;
	public int QuantidadeAulasNoCurso { get; set; }
	public int QuantidadeAulasRealizadas { get; set; }
	public int QuantidadeAulasEmAndamento { get; set; }

	public static implicit operator EvolucaoMatriculaCursoViewModel(EvolucaoMatriculaCursoDto dto)
	{
		return new EvolucaoMatriculaCursoViewModel
		{
			Id = dto.Id,
			CursoId = dto.CursoId,
			NomeCurso = dto.NomeCurso,
			Valor = dto.Valor,
			DataMatricula = dto.DataMatricula,
			DataConclusao = dto.DataConclusao,
			EstadoMatricula = dto.EstadoMatricula,
			Certificado = dto.Certificado,
			QuantidadeAulasNoCurso = dto.QuantidadeAulasNoCurso,
			QuantidadeAulasRealizadas = dto.QuantidadeAulasRealizadas,
			QuantidadeAulasEmAndamento = dto.QuantidadeAulasEmAndamento
		};
	}
}