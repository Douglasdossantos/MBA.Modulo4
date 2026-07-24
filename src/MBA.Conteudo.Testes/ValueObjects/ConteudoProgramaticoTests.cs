using FluentAssertions;

using MBA.Conteudo.Domain.ValueObjects;
using MBA.Core.DomainObjects;

namespace MBA.Conteudo.Testes.ValueObjects;

public class ConteudoProgramaticoTests
{
	private const string FinalidadeValida = "Finalidade valida do conteudo";
	private const string EmentaValida = "Ementa valida do conteudo programatico";

	[Fact]
	public void Deve_criar_conteudo_programatico_valido()
	{
		var conteudo = new ConteudoProgramatico(FinalidadeValida, EmentaValida);

		conteudo.Should().NotBeNull();
		conteudo.Finalidade.Should().Be(FinalidadeValida);
		conteudo.Ementa.Should().Be(EmentaValida);
	}

	[Theory]
	[InlineData("", "*Finalidade não pode ser vazia*")]
	[InlineData("abc", "*Finalidade do conteúdo programático deve ter entre*")]
	public void Nao_deve_criar_com_finalidade_invalida(string finalidade, string mensagem)
	{
		Action act = () => new ConteudoProgramatico(finalidade, EmentaValida);

		act.Should().Throw<DomainException>().WithMessage(mensagem);
	}

	[Theory]
	[InlineData("", "*Ementa do conteúdo programático não pode ser vazia*")]
	[InlineData("abc", "*Ementa do conteúdo programático deve ter entre*")]
	public void Nao_deve_criar_com_ementa_invalida(string ementa, string mensagem)
	{
		Action act = () => new ConteudoProgramatico(FinalidadeValida, ementa);

		act.Should().Throw<DomainException>().WithMessage(mensagem);
	}
}
