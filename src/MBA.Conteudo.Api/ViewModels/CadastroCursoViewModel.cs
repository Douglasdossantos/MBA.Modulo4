using MBA.Conteudo.Application.ViewModels;

namespace MBA.Conteudo.Api.ViewModels;

public class CadastroCursoViewModel
{
	public string Nome { get; set; } = string.Empty;
	public decimal Valor { get; set; }
	public DateTime? ValidoAte { get; set; }

	public string Finalidade { get; set; } = string.Empty;
	public string Ementa { get; set; } = string.Empty;

	public static implicit operator CadastroCursoDto(CadastroCursoViewModel viewModel)
	{
		return new CadastroCursoDto
		{
			Nome = viewModel.Nome,
			Valor = viewModel.Valor,
			ValidoAte = viewModel.ValidoAte,
			Finalidade = viewModel.Finalidade,
			Ementa = viewModel.Ementa
		};
	}
}