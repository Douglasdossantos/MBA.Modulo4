using MBA.Core.SharedDto;

namespace MBA.Conteudo.Api.ViewModels;

public class CertificadoViewModel
{
	public Guid Id { get; set; }
	public DateTime DataSolicitacao { get; set; }
	public string PathCertificado { get; set; } = string.Empty;

	public static implicit operator CertificadoViewModel(CertificadoDto dto)
	{
		return new CertificadoViewModel
		{
			Id = dto.Id,
			DataSolicitacao = dto.DataSolicitacao,
			PathCertificado = dto.PathCertificado
		};
	}
}