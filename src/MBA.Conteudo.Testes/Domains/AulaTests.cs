using FluentAssertions;

using MBA.Conteudo.Domain.Entities;
using MBA.Core.DomainObjects;

namespace MBA.Conteudo.Testes.Domains;

public class AulaTests
{
	#region Helpers

	private static readonly Guid CursoIdValido = Guid.NewGuid();

	private static Aula CriarAula(
		Guid? cursoId = null,
		string descricao = "Aula introdutoria",
		short cargaHoraria = 3,
		byte ordemAula = 1,
		string url = "http://curso.com/aula-intro")
		=> new(cursoId ?? CursoIdValido, descricao, cargaHoraria, ordemAula, url);

	#endregion

	[Fact]
	public void Deve_criar_aula_valida()
	{
		var aula = CriarAula();

		aula.Should().NotBeNull();
		aula.CursoId.Should().Be(CursoIdValido);
		aula.Descricao.Should().Be("Aula introdutoria");
		aula.CargaHoraria.Should().Be((short)3);
		aula.OrdemAula.Should().Be((byte)1);
		aula.Ativo.Should().BeTrue();
	}

	[Fact]
	public void Deve_criar_aula_com_id_informado()
	{
		var aulaId = Guid.NewGuid();

		var aula = new Aula(CursoIdValido, aulaId, "Aula com id fixo", 2, 1, "http://curso.com/aula-id");

		aula.Id.Should().Be(aulaId);
		aula.CursoId.Should().Be(CursoIdValido);
	}

	[Fact]
	public void Nao_deve_criar_aula_com_curso_vazio()
	{
		Action act = () => CriarAula(cursoId: Guid.Empty);

		act.Should().Throw<DomainException>()
			.WithMessage("*Id do curso não pode ser vazio*");
	}

	[Theory]
	[InlineData("", "*Descrição da aula não pode ser vazia*")]
	[InlineData("Abc", "*Descrição da aula deve ter entre 5 e 100*")]
	public void Nao_deve_criar_aula_com_descricao_invalida(string descricao, string mensagem)
	{
		Action act = () => CriarAula(descricao: descricao);

		act.Should().Throw<DomainException>().WithMessage(mensagem);
	}

	[Fact]
	public void Nao_deve_criar_aula_com_carga_horaria_zero()
	{
		Action act = () => CriarAula(cargaHoraria: 0);

		act.Should().Throw<DomainException>()
			.WithMessage("*Carga horária deve ser maior que zero*");
	}

	[Fact]
	public void Nao_deve_criar_aula_com_carga_horaria_acima_do_limite()
	{
		Action act = () => CriarAula(cargaHoraria: 6);

		act.Should().Throw<DomainException>()
			.WithMessage("*entre 1 e 5 horas*");
	}

	[Fact]
	public void Nao_deve_criar_aula_com_ordem_zero()
	{
		Action act = () => CriarAula(ordemAula: 0);

		act.Should().Throw<DomainException>()
			.WithMessage("*Ordem da aula deve ser maior que zero*");
	}

	[Theory]
	[InlineData("", "*URL da aula não pode ser vazia*")]
	[InlineData("http", "*Url da aula deve ter entre 10 e 1024*")]
	public void Nao_deve_criar_aula_com_url_invalida(string url, string mensagem)
	{
		Action act = () => CriarAula(url: url);

		act.Should().Throw<DomainException>().WithMessage(mensagem);
	}

	[Fact]
	public void ToString_deve_conter_a_descricao()
	{
		var aula = CriarAula();

		aula.ToString().Should().Contain("Aula introdutoria");
	}
}
