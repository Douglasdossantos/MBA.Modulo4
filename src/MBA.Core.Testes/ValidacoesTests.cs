using FluentAssertions;

using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;

namespace MBA.Core.Testes;

public class ValidacoesTests
{
	private const string Msg = "erro";

	[Fact]
	public void ValidarSeIgual_deve_lancar_quando_iguais_e_passar_quando_diferentes()
	{
		((Action)(() => Validacoes.ValidarSeIgual((object)1, (object)1, Msg)))
			.Should().Throw<DomainException>().WithMessage(Msg);
		((Action)(() => Validacoes.ValidarSeIgual((object)1, (object)2, Msg)))
			.Should().NotThrow();
	}

	[Fact]
	public void ValidarSeDiferente_objeto_deve_lancar_quando_diferentes_e_passar_quando_iguais()
	{
		((Action)(() => Validacoes.ValidarSeDiferente((object)1, (object)2, Msg)))
			.Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeDiferente((object)1, (object)1, Msg)))
			.Should().NotThrow();
	}

	[Fact]
	public void ValidarSeDiferente_regex_deve_lancar_quando_nao_casa_e_passar_quando_casa()
	{
		((Action)(() => Validacoes.ValidarSeDiferente("^\\d+$", "abc", Msg)))
			.Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeDiferente("^\\d+$", "123", Msg)))
			.Should().NotThrow();
	}

	[Fact]
	public void ValidarTamanho_maximo_deve_lancar_quando_excede()
	{
		((Action)(() => Validacoes.ValidarTamanho("abcd", 3, Msg)))
			.Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarTamanho("abc", 3, Msg)))
			.Should().NotThrow();
	}

	[Fact]
	public void ValidarTamanho_minimo_maximo_deve_lancar_fora_do_range()
	{
		((Action)(() => Validacoes.ValidarTamanho("ab", 3, 5, Msg)))
			.Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarTamanho("abcdef", 3, 5, Msg)))
			.Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarTamanho("abcd", 3, 5, Msg)))
			.Should().NotThrow();
	}

	[Fact]
	public void ValidarSeVazio_string_deve_lancar_para_nulo_ou_vazio()
	{
		((Action)(() => Validacoes.ValidarSeVazio((string)null!, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeVazio("   ", Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeVazio("x", Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeVazio_guid_deve_lancar_para_guid_vazio()
	{
		((Action)(() => Validacoes.ValidarSeVazio(Guid.Empty, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeVazio(Guid.NewGuid(), Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeNulo_deve_lancar_para_nulo()
	{
		((Action)(() => Validacoes.ValidarSeNulo(null!, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeNulo(new object(), Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarMinimoMaximo_double_fora_do_range_deve_lancar()
	{
		((Action)(() => Validacoes.ValidarMinimoMaximo(5.0, 0.0, 3.0, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarMinimoMaximo(2.0, 0.0, 3.0, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarMinimoMaximo_float_fora_do_range_deve_lancar()
	{
		((Action)(() => Validacoes.ValidarMinimoMaximo(5f, 0f, 3f, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarMinimoMaximo(2f, 0f, 3f, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarMinimoMaximo_int_fora_do_range_deve_lancar()
	{
		((Action)(() => Validacoes.ValidarMinimoMaximo(5, 0, 3, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarMinimoMaximo(2, 0, 3, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarMinimoMaximo_long_fora_do_range_deve_lancar()
	{
		((Action)(() => Validacoes.ValidarMinimoMaximo(5L, 0L, 3L, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarMinimoMaximo(2L, 0L, 3L, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarMinimoMaximo_decimal_fora_do_range_deve_lancar()
	{
		((Action)(() => Validacoes.ValidarMinimoMaximo(5m, 0m, 3m, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarMinimoMaximo(2m, 0m, 3m, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarMinimoMaximos_short_fora_do_range_deve_lancar()
	{
		((Action)(() => Validacoes.ValidarMinimoMaximos((short)5, (short)0, (short)3, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarMinimoMaximos((short)2, (short)0, (short)3, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeMenorQue_long_deve_lancar_quando_menor()
	{
		((Action)(() => Validacoes.ValidarSeMenorQue(1L, 5L, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeMenorQue(9L, 5L, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeMenorQue_double_deve_lancar_quando_menor()
	{
		((Action)(() => Validacoes.ValidarSeMenorQue(1.0, 5.0, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeMenorQue(9.0, 5.0, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeMenorQue_decimal_deve_lancar_quando_menor()
	{
		((Action)(() => Validacoes.ValidarSeMenorQue(1m, 5m, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeMenorQue(9m, 5m, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeMenorQue_int_deve_lancar_quando_menor()
	{
		((Action)(() => Validacoes.ValidarSeMenorQue(1, 5, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeMenorQue(9, 5, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeMenorQuee_short_deve_lancar_quando_menor()
	{
		((Action)(() => Validacoes.ValidarSeMenorQuee((short)1, (short)5, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeMenorQuee((short)9, (short)5, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeMenorQuee_decimal_deve_lancar_quando_menor()
	{
		((Action)(() => Validacoes.ValidarSeMenorQuee(1m, 5m, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeMenorQuee(9m, 5m, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeMenorQuee_byte_deve_lancar_quando_menor()
	{
		((Action)(() => Validacoes.ValidarSeMenorQuee((byte)1, (byte)5, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeMenorQuee((byte)9, (byte)5, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeFalso_deve_lancar_quando_false()
	{
		((Action)(() => Validacoes.ValidarSeFalso(false, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeFalso(true, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarSeVerdadeiro_deve_lancar_quando_true()
	{
		((Action)(() => Validacoes.ValidarSeVerdadeiro(true, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarSeVerdadeiro(false, Msg))).Should().NotThrow();
	}

	[Fact]
	public void ValidarData_deve_lancar_para_data_default()
	{
		((Action)(() => Validacoes.ValidarData(default, Msg))).Should().Throw<DomainException>();
		((Action)(() => Validacoes.ValidarData(new DateTime(2020, 1, 1), Msg))).Should().NotThrow();
	}
}
