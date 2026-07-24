using FluentAssertions;

using MBA.WebApi.Core.Extensions;

using Microsoft.Extensions.Configuration;

namespace MBA.WebApi.Core.Testes;

public class ValidacaoConfiguracaoExtensionTests
{
	private static IConfiguration Config(params (string chave, string? valor)[] pares)
		=> new ConfigurationBuilder()
			.AddInMemoryCollection(pares.Select(p => new KeyValuePair<string, string?>(p.chave, p.valor)))
			.Build();

	[Fact]
	public void Todos_os_segredos_presentes_nao_deve_lancar()
	{
		var config = Config(("Jwt:Secret", "abc"), ("ConnectionStrings:Default", "server=."));

		var acao = () => config.ValidarSegredosObrigatorios("Jwt:Secret", "ConnectionStrings:Default");

		acao.Should().NotThrow();
	}

	[Fact]
	public void Sem_chaves_exigidas_nao_deve_lancar()
	{
		var config = Config();

		var acao = () => config.ValidarSegredosObrigatorios();

		acao.Should().NotThrow();
	}

	[Fact]
	public void Chave_ausente_deve_lancar_com_a_chave_na_mensagem()
	{
		var config = Config(("Jwt:Secret", "abc"));

		var acao = () => config.ValidarSegredosObrigatorios("Jwt:Secret", "ConnectionStrings:Default");

		acao.Should().Throw<InvalidOperationException>()
			.WithMessage("*ConnectionStrings:Default*");
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void Valor_em_branco_ou_nulo_deve_ser_tratado_como_ausente(string? valor)
	{
		var config = Config(("Jwt:Secret", valor));

		var acao = () => config.ValidarSegredosObrigatorios("Jwt:Secret");

		acao.Should().Throw<InvalidOperationException>()
			.WithMessage("*Jwt:Secret*");
	}

	[Fact]
	public void Multiplas_ausentes_devem_aparecer_todas_na_mensagem()
	{
		var config = Config(("Presente", "ok"));

		var acao = () => config.ValidarSegredosObrigatorios("FaltaUm", "FaltaDois", "Presente");

		acao.Should().Throw<InvalidOperationException>()
			.Which.Message.Should().Contain("FaltaUm").And.Contain("FaltaDois");
	}
}
