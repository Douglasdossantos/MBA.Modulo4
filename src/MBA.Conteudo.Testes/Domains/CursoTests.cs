using FluentAssertions;

using MBA.Conteudo.Domain.Entities;
using MBA.Conteudo.Domain.ValueObjects;
using MBA.Core.DomainObjects;

namespace MBA.Conteudo.Testes.Domains;

public class CursoTests
{
	#region Helpers

	private const string NomeValido = "Curso de Teste Valido";

	private static ConteudoProgramatico CriarConteudo()
		=> new("Finalidade do curso valida", "Ementa do curso com conteudo valido");

	private static Curso CriarCurso(
		string nome = NomeValido,
		decimal valor = 100m,
		DateTime? validoAte = null,
		ConteudoProgramatico? conteudo = null)
		=> new(nome, valor, validoAte, conteudo ?? CriarConteudo());

	private static Guid AdicionarAulaEObterId(Curso curso,
		string descricao = "Aula introdutoria do curso",
		short carga = 2,
		byte ordem = 1,
		string url = "http://curso.com/aula-intro")
	{
		curso.AdicionarAula(descricao, carga, ordem, url);
		return curso.Aulas.Last().Id;
	}

	#endregion

	#region Construtor

	[Fact]
	public void Deve_criar_curso_valido()
	{
		var curso = CriarCurso();

		curso.Should().NotBeNull();
		curso.Nome.Should().Be(NomeValido);
		curso.Valor.Should().Be(100m);
		curso.Ativo.Should().BeTrue();
		curso.Aulas.Should().BeEmpty();
		curso.QuantidadeAulas().Should().Be(0);
		curso.ConteudoProgramatico.Should().NotBeNull();
	}

	[Fact]
	public void Nao_deve_criar_curso_com_nome_vazio()
	{
		Action act = () => CriarCurso(nome: "");

		act.Should().Throw<DomainException>()
			.WithMessage("*Nome do curso não pode ser vazio*");
	}

	[Fact]
	public void Nao_deve_criar_curso_com_nome_curto()
	{
		Action act = () => CriarCurso(nome: "Curso");

		act.Should().Throw<DomainException>()
			.WithMessage("*Nome do curso deve ter entre 10 e 100*");
	}

	[Fact]
	public void Nao_deve_criar_curso_com_valor_zero()
	{
		Action act = () => CriarCurso(valor: 0m);

		act.Should().Throw<DomainException>()
			.WithMessage("*Valor do curso deve ser maior que zero*");
	}

	[Fact]
	public void Nao_deve_criar_curso_sem_conteudo_programatico()
	{
		Action act = () => new Curso(NomeValido, 100m, null, null!);

		act.Should().Throw<DomainException>()
			.WithMessage("*Conteúdo programático não foi informado*");
	}

	[Fact]
	public void Nao_deve_criar_curso_com_validade_invalida()
	{
		Action act = () => CriarCurso(validoAte: DateTime.MinValue);

		act.Should().Throw<DomainException>()
			.WithMessage("*Data de validade*");
	}

	#endregion

	#region Estado do curso

	[Fact]
	public void Deve_ativar_e_desativar_curso()
	{
		var curso = CriarCurso();

		curso.DesativarCurso();
		curso.Ativo.Should().BeFalse();

		curso.AtivarCurso();
		curso.Ativo.Should().BeTrue();
	}

	[Fact]
	public void Curso_ativo_sem_validade_deve_estar_disponivel()
	{
		var curso = CriarCurso();

		curso.CursoDisponivel().Should().BeTrue();
	}

	[Fact]
	public void Curso_desativado_nao_deve_estar_disponivel()
	{
		var curso = CriarCurso();

		curso.DesativarCurso();

		curso.CursoDisponivel().Should().BeFalse();
	}

	[Fact]
	public void Curso_com_validade_no_passado_nao_deve_estar_disponivel()
	{
		var curso = CriarCurso();

		curso.AlterarValidadeCurso(DateTime.Now.AddDays(-1));

		curso.CursoDisponivel().Should().BeFalse();
	}

	#endregion

	#region Alteracoes

	[Fact]
	public void Deve_alterar_nome_valor_e_validade()
	{
		var curso = CriarCurso();
		var novaValidade = DateTime.Now.AddDays(30);

		curso.AlterarNome("Novo Nome do Curso");
		curso.AlterarValor(250m);
		curso.AlterarValidadeCurso(novaValidade);

		curso.Nome.Should().Be("Novo Nome do Curso");
		curso.Valor.Should().Be(250m);
		curso.ValidoAte.Should().Be(novaValidade);
	}

	[Fact]
	public void Nao_deve_alterar_nome_para_valor_curto()
	{
		var curso = CriarCurso();

		Action act = () => curso.AlterarNome("Curto");

		act.Should().Throw<DomainException>()
			.WithMessage("*Nome do curso deve ter entre 10 e 100*");
	}

	[Fact]
	public void Nao_deve_alterar_valor_para_zero()
	{
		var curso = CriarCurso();

		Action act = () => curso.AlterarValor(0m);

		act.Should().Throw<DomainException>()
			.WithMessage("*Valor do curso deve ser maior que zero*");
	}

	[Fact]
	public void Deve_atualizar_conteudo_programatico()
	{
		var curso = CriarCurso();

		curso.AtualizarConteudoProgramatico("Nova finalidade", "Nova ementa do curso");

		curso.ConteudoProgramatico.Finalidade.Should().Be("Nova finalidade");
		curso.ConteudoProgramatico.Ementa.Should().Be("Nova ementa do curso");
	}

	[Fact]
	public void Nao_deve_atualizar_conteudo_programatico_invalido()
	{
		var curso = CriarCurso();

		Action act = () => curso.AtualizarConteudoProgramatico("", "Ementa valida do curso");

		act.Should().Throw<DomainException>()
			.WithMessage("*Finalidade não pode ser vazia*");
	}

	#endregion

	#region Aulas

	[Fact]
	public void Deve_adicionar_aula_e_calcular_carga_horaria()
	{
		var curso = CriarCurso();

		curso.AdicionarAula("Aula um bem descrita", 2, 1, "http://curso.com/aula1");
		curso.AdicionarAula("Aula dois bem descrita", 3, 2, "http://curso.com/aula2");

		curso.QuantidadeAulas().Should().Be(2);
		curso.CargaHoraria().Should().Be((short)5);
		curso.Aulas.Should().HaveCount(2);
	}

	[Fact]
	public void Nao_deve_adicionar_aula_com_ordem_duplicada()
	{
		var curso = CriarCurso();
		curso.AdicionarAula("Aula um bem descrita", 2, 1, "http://curso.com/aula1");

		Action act = () => curso.AdicionarAula("Aula dois bem descrita", 3, 1, "http://curso.com/aula2");

		act.Should().Throw<DomainException>()
			.WithMessage("*Já existe uma aula com a ordem*");
	}

	[Fact]
	public void Deve_obter_aula_pelo_id()
	{
		var curso = CriarCurso();
		var aulaId = AdicionarAulaEObterId(curso);

		var aula = curso.ObterAulaPeloId(aulaId);

		aula.Should().NotBeNull();
		aula.Id.Should().Be(aulaId);
	}

	[Fact]
	public void Deve_lancar_ao_obter_aula_inexistente()
	{
		var curso = CriarCurso();

		Action act = () => curso.ObterAulaPeloId(Guid.NewGuid());

		act.Should().Throw<DomainException>()
			.WithMessage("*Aula não encontrada*");
	}

	[Fact]
	public void Deve_remover_aula_do_curso()
	{
		var curso = CriarCurso();
		AdicionarAulaEObterId(curso);
		var aula = curso.Aulas.Last();

		curso.RemoverAula(aula);

		curso.QuantidadeAulas().Should().Be(0);
	}

	[Fact]
	public void Nao_deve_remover_aula_de_outro_curso()
	{
		var curso = CriarCurso();
		var aulaDeFora = new Aula(Guid.NewGuid(), "Aula de outro curso", 2, 1, "http://curso.com/outra");

		Action act = () => curso.RemoverAula(aulaDeFora);

		act.Should().Throw<DomainException>()
			.WithMessage("*Aula não pertence a este curso*");
	}

	[Fact]
	public void Deve_ativar_e_desativar_aula_pelo_curso()
	{
		var curso = CriarCurso();
		var aulaId = AdicionarAulaEObterId(curso);

		curso.DesativarAula(aulaId);
		curso.Aulas.Last().Ativo.Should().BeFalse();

		curso.AtivarAula(aulaId);
		curso.Aulas.Last().Ativo.Should().BeTrue();
	}

	[Fact]
	public void Deve_alterar_dados_da_aula_pelo_curso()
	{
		var curso = CriarCurso();
		var aulaId = AdicionarAulaEObterId(curso);

		curso.AlterarDescricaoAula(aulaId, "Descricao alterada da aula");
		curso.AlterarCargaHorariaAula(aulaId, 4);
		curso.AlterarOrdemAula(aulaId, 3);
		curso.AlterarUrlAula(aulaId, "http://curso.com/aula-alterada");

		var aula = curso.Aulas.Last();
		aula.Descricao.Should().Be("Descricao alterada da aula");
		aula.CargaHoraria.Should().Be((short)4);
		aula.OrdemAula.Should().Be((byte)3);
		aula.Url.Should().Be("http://curso.com/aula-alterada");
	}

	[Fact]
	public void ToString_deve_conter_o_nome()
	{
		var curso = CriarCurso();

		curso.ToString().Should().Contain(NomeValido);
	}

	#endregion
}
