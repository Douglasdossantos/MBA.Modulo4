using MBA.Core.DomainValidations;

namespace MBA.Conteudo.Domain.ValueObjects;

public class ConteudoProgramatico
{
	public string Finalidade { get; private set; } = null!;
	public string Ementa { get; private set; } = null!;

	// EF Constructor
	protected ConteudoProgramatico() { }

	public ConteudoProgramatico(string finalidade, string ementa)
	{
		Finalidade = finalidade;
		Ementa = ementa;

		ValidarIntegridadeConteudoProgramatico(finalidade, ementa);
	}

	private void ValidarIntegridadeConteudoProgramatico(string novaFinalidade = "", string novaEmenta = "")
	{
		var finalidade = string.IsNullOrEmpty(novaFinalidade) ? Finalidade : novaFinalidade;
		var ementa = string.IsNullOrEmpty(novaEmenta) ? Ementa : novaEmenta;

		var validacao = new ResultadoValidacao<ConteudoProgramatico>();
		ValidacaoTexto.DevePossuirConteudo(finalidade, "Finalidade não pode ser vazia ou nula", validacao);
		ValidacaoTexto.DevePossuirTamanho(finalidade, 5, 4000,
			"Finalidade do conteúdo programático deve ter entre 10 e 100 caracteres", validacao);
		ValidacaoTexto.DevePossuirConteudo(ementa, "Ementa do conteúdo programático não pode ser vazia ou nula",
			validacao);
		ValidacaoTexto.DevePossuirTamanho(ementa, 5, 4000,
			"Ementa do conteúdo programático deve ter entre 50 e 4000 caracteres", validacao);

		validacao.DispararExcecaoDominioSeInvalido();
	}
}