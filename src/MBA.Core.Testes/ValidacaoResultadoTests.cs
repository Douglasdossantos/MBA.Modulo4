using FluentAssertions;

using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;

namespace MBA.Core.Testes;

// Tipo alvo de referencia usado nas validacoes baseadas em ResultadoValidacao<T>.
internal sealed class AlvoValidacao { }

public class ResultadoValidacaoTests
{
	[Fact]
	public void Sem_erros_nao_deve_lancar()
	{
		var resultado = new ResultadoValidacao<AlvoValidacao>();

		((Action)resultado.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void Com_erro_deve_lancar_e_prefixar_com_o_tipo()
	{
		var resultado = new ResultadoValidacao<AlvoValidacao>();
		resultado.AdicionarErro("falha");

		((Action)resultado.DispararExcecaoDominioSeInvalido)
			.Should().Throw<DomainException>()
			.Which.Errors.Should().Contain("(AlvoValidacao) falha");
	}

	[Fact]
	public void Erro_em_branco_ou_vazio_deve_ser_ignorado()
	{
		var resultado = new ResultadoValidacao<AlvoValidacao>();
		resultado.AdicionarErro("   ");
		resultado.AdicionarErro("");

		((Action)resultado.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}
}

public class ValidacaoTextoTests
{
	private static ResultadoValidacao<AlvoValidacao> Novo() => new();

	[Fact]
	public void DeveSerDiferenteDe_adiciona_erro_quando_diferentes()
	{
		var r = Novo();
		ValidacaoTexto.DeveSerDiferenteDe("a", "b", "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoTexto.DeveSerDiferenteDe("a", "a", "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void DevePossuirConteudo_adiciona_erro_quando_vazio()
	{
		var r = Novo();
		ValidacaoTexto.DevePossuirConteudo("   ", "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoTexto.DevePossuirConteudo("conteudo", "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void DevePossuirTamanho_adiciona_erro_fora_do_range()
	{
		var curto = Novo();
		ValidacaoTexto.DevePossuirTamanho("ab", 3, 5, "erro", curto);
		((Action)curto.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var longo = Novo();
		ValidacaoTexto.DevePossuirTamanho("abcdef", 1, 3, "erro", longo);
		((Action)longo.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoTexto.DevePossuirTamanho("abcd", 3, 5, "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void DeveAtenderRegex_adiciona_erro_quando_nao_casa_ou_nulo()
	{
		var naoCasa = Novo();
		ValidacaoTexto.DeveAtenderRegex("abc", "^\\d+$", "erro", naoCasa);
		((Action)naoCasa.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var nulo = Novo();
		ValidacaoTexto.DeveAtenderRegex(null, "^\\d+$", "erro", nulo);
		((Action)nulo.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoTexto.DeveAtenderRegex("123", "^\\d+$", "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}
}

public class ValidacaoDataTests
{
	private static ResultadoValidacao<AlvoValidacao> Novo() => new();

	[Fact]
	public void DeveSerValido_adiciona_erro_para_data_limite()
	{
		var min = Novo();
		ValidacaoData.DeveSerValido(DateTime.MinValue, "erro", min);
		((Action)min.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var max = Novo();
		ValidacaoData.DeveSerValido(DateTime.MaxValue, "erro", max);
		((Action)max.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoData.DeveSerValido(new DateTime(2020, 1, 1), "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void DeveSerMenorQue_adiciona_erro_quando_maior()
	{
		var r = Novo();
		ValidacaoData.DeveSerMenorQue(new DateTime(2021, 1, 1), new DateTime(2020, 1, 1), "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoData.DeveSerMenorQue(new DateTime(2019, 1, 1), new DateTime(2020, 1, 1), "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void DeveSerMaiorQue_adiciona_erro_quando_menor()
	{
		var r = Novo();
		ValidacaoData.DeveSerMaiorQue(new DateTime(2019, 1, 1), new DateTime(2020, 1, 1), "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoData.DeveSerMaiorQue(new DateTime(2021, 1, 1), new DateTime(2020, 1, 1), "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void DeveTerRangeValido_adiciona_erro_quando_inicio_apos_fim()
	{
		var r = Novo();
		ValidacaoData.DeveTerRangeValido(new DateTime(2021, 1, 1), new DateTime(2020, 1, 1), "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoData.DeveTerRangeValido(new DateTime(2020, 1, 1), new DateTime(2021, 1, 1), "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}
}

public class ValidacaoNumericaTests
{
	private static ResultadoValidacao<AlvoValidacao> Novo() => new();

	[Fact]
	public void DeveSerMaiorQueZero_byte()
	{
		var r = Novo();
		ValidacaoNumerica.DeveSerMaiorQueZero((byte)0, "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoNumerica.DeveSerMaiorQueZero((byte)1, "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void DeveSerMaiorQueZero_short()
	{
		var r = Novo();
		ValidacaoNumerica.DeveSerMaiorQueZero((short)0, "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoNumerica.DeveSerMaiorQueZero((short)1, "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void DeveSerMaiorQueZero_int()
	{
		var r = Novo();
		ValidacaoNumerica.DeveSerMaiorQueZero(0, "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoNumerica.DeveSerMaiorQueZero(1, "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void DeveSerMaiorQueZero_decimal()
	{
		var r = Novo();
		ValidacaoNumerica.DeveSerMaiorQueZero(0m, "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoNumerica.DeveSerMaiorQueZero(1m, "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void DeveEstarEntre_int_fora_do_range()
	{
		var r = Novo();
		ValidacaoNumerica.DeveEstarEntre(10, 1, 5, "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoNumerica.DeveEstarEntre(3, 1, 5, "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}
}

public class ValidacaoObjetoEGuidTests
{
	private static ResultadoValidacao<AlvoValidacao> Novo() => new();

	[Fact]
	public void DeveEstarInstanciado_adiciona_erro_para_nulo()
	{
		var r = Novo();
		ValidacaoObjeto.DeveEstarInstanciado(null, "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoObjeto.DeveEstarInstanciado(new object(), "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}

	[Fact]
	public void Guid_DeveSerValido_adiciona_erro_para_guid_vazio()
	{
		var r = Novo();
		ValidacaoGuid.DeveSerValido(Guid.Empty, "erro", r);
		((Action)r.DispararExcecaoDominioSeInvalido).Should().Throw<DomainException>();

		var ok = Novo();
		ValidacaoGuid.DeveSerValido(Guid.NewGuid(), "erro", ok);
		((Action)ok.DispararExcecaoDominioSeInvalido).Should().NotThrow();
	}
}

public class DomainExceptionTests
{
	[Fact]
	public void Ctor_com_mensagem_expoe_a_mensagem_e_um_erro()
	{
		var ex = new DomainException("boom");

		ex.Message.Should().Be("boom");
		ex.Errors.Should().ContainSingle().Which.Should().Be("boom");
	}

	[Fact]
	public void Ctor_com_lista_junta_mensagens_e_preserva_erros()
	{
		var ex = new DomainException(new[] { "a", "b" });

		ex.Errors.Should().BeEquivalentTo(new[] { "a", "b" });
		ex.Message.Should().Contain("a").And.Contain("b");
	}
}
