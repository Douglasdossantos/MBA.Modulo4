using MBA.Conteudo.Application.ViewModels;

namespace MBA.Conteudo.Api.ViewModels;

public class AtualizacaoCursoViewModel
{
	public Guid Id { get; set; }
	public string Nome { get; set; } = string.Empty;
	public decimal Valor { get; set; }
	public DateTime? ValidoAte { get; set; }
	public bool Ativo { get; set; }
	public string Finalidade { get; set; } = string.Empty;
	public string Ementa { get; set; } = string.Empty;

	public static implicit operator AtualizacaoCursoDto(AtualizacaoCursoViewModel viewModel)
	{
		return new AtualizacaoCursoDto
		{
			Id = viewModel.Id,
			Nome = viewModel.Nome,
			Valor = viewModel.Valor,
			ValidoAte = viewModel.ValidoAte,
			Ativo = viewModel.Ativo,
			Finalidade = viewModel.Finalidade,
			Ementa = viewModel.Ementa
		};
	}
}