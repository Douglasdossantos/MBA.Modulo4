using MBA.Core.SharedDto;

namespace MBA.Conteudo.Api.ViewModels;

public class AulaViewModel
{
	public Guid Id { get; set; }
	public Guid CursoId { get; set; }
	public string Descricao { get; set; } = string.Empty;
	public short CargaHoraria { get; set; }
	public byte OrdemAula { get; set; }
	public bool Ativo { get; set; }
	public string Url { get; set; } = string.Empty;

	public static implicit operator AulaDto(AulaViewModel viewModel)
	{
		return new AulaDto
		{
			Id = viewModel.Id,
			CursoId = viewModel.CursoId,
			Descricao = viewModel.Descricao,
			CargaHoraria = viewModel.CargaHoraria,
			OrdemAula = viewModel.OrdemAula,
			Ativo = viewModel.Ativo,
			Url = viewModel.Url
		};
	}
}